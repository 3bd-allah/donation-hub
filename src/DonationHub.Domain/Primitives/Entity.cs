using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Primitives
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; private set; }

        protected Entity(Guid id) => Id = id;
        protected Entity()
        {
            Id = Guid.CreateVersion7(); 
        }

        public override bool Equals(object? obj)
        {
            if(obj is null)
                return false;

            if(obj.GetType() != GetType())
                return false;

            if (obj is not Entity other)
                return false;

            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * 41;
        }   
        public bool Equals(Entity? other)
        {
            if(other is null || other.GetType() != GetType())
                return false;
            
            return other.Id == Id;
        }

    }
}
