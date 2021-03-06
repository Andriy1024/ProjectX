﻿using System;

namespace ProjectX.Identity.Domain
{
    public sealed class SessionEntity
    {
        public string Id { get; private set; }
        public long UserId { get; private set; }
        public UserEntity User { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        
        public SessionLifetime Lifetime { get; private set; }

        public bool IsActive { get; private set; }

        private SessionEntity() { }

        public SessionEntity(UserEntity user, Guid id, DateTime createdAt)
        {
            Id = id.ToString();
            UserId = user.Id;
            User = user;
            UpdatedAt = CreatedAt = createdAt;
            Lifetime = SessionLifetime.Create(CreatedAt);
            IsActive = true;
        }

        public void Deactivate(DateTime deactivationTime)
        {
            IsActive = false;
            UpdatedAt = deactivationTime;
        }

        public void RefreshLifetime(DateTime refreshTime)
        {
            if (!IsActive)
                throw new InvalidOperationException("Inactive session can not be refreshed.");

            UpdatedAt = refreshTime;
            Lifetime = SessionLifetime.Create(UpdatedAt);
        }
    }
}
