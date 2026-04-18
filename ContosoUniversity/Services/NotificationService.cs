using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ContosoUniversity.Models;
using Newtonsoft.Json;

namespace ContosoUniversity.Services
{
    /// <summary>
    /// NotificationService - Handles entity operation notifications
    /// In .NET Core, this uses an in-memory queue. For production, consider using:
    /// - Azure Service Bus
    /// - RabbitMQ
    /// - Kafka
    /// </summary>
    public class NotificationService
    {
        private static readonly Queue<Notification> _notificationQueue = new Queue<Notification>();
        private readonly IConfiguration _configuration;

        public NotificationService()
        {
            _configuration = null;
        }

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendNotification(string entityType, string entityId, EntityOperation operation, string userName = null)
        {
            SendNotification(entityType, entityId, null, operation, userName);
        }

        public void SendNotification(string entityType, string entityId, string entityDisplayName, EntityOperation operation, string userName = null)
        {
            try
            {
                var notification = new Notification
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Operation = operation.ToString(),
                    Message = GenerateMessage(entityType, entityId, entityDisplayName, operation),
                    CreatedAt = DateTime.Now,
                    CreatedBy = userName ?? "System",
                    IsRead = false
                };

                lock (_notificationQueue)
                {
                    _notificationQueue.Enqueue(notification);
                    
                    // Keep queue size manageable - remove old messages if exceeds limit
                    if (_notificationQueue.Count > 100)
                    {
                        _notificationQueue.Dequeue();
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Notification queued: {entityType} {operation}");
            }
            catch (Exception ex)
            {
                // Log error but don't break the main operation
                System.Diagnostics.Debug.WriteLine($"Failed to send notification: {ex.Message}");
            }
        }

        public Notification ReceiveNotification()
        {
            try
            {
                lock (_notificationQueue)
                {
                    if (_notificationQueue.Count > 0)
                    {
                        return _notificationQueue.Dequeue();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to receive notification: {ex.Message}");
                return null;
            }
        }

        public void MarkAsRead(int notificationId)
        {
            // In a real implementation, you might want to store notifications in database as well
            // for persistence and tracking read status
        }

        private string GenerateMessage(string entityType, string entityId, string entityDisplayName, EntityOperation operation)
        {
            var displayText = !string.IsNullOrWhiteSpace(entityDisplayName) 
                ? $"{entityType} '{entityDisplayName}'" 
                : $"{entityType} (ID: {entityId})";

            switch (operation)
            {
                case EntityOperation.CREATE:
                    return $"New {displayText} has been created";
                case EntityOperation.UPDATE:
                    return $"{displayText} has been updated";
                case EntityOperation.DELETE:
                    return $"{displayText} has been deleted";
                default:
                    return $"{displayText} operation: {operation}";
            }
        }

        public void Dispose()
        {
            // Nothing to dispose in in-memory queue implementation
        }
    }
}
