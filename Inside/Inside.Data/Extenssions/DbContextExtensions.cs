using Inside.Data.Helper;
using Inside.Domain.Core;
using Inside.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace Inside.Data.Extenssions
{
    public static class DbContextExtensions
    {
        public static void ApplyChanges(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<IObjectWithState>())
            {
                var objectWithState = entry.Entity;
                if (objectWithState.ObjectState == ObjectState.PartiallyModified)
                {
                    HandlePartialUpdate(context, objectWithState, entry);
                }
                else
                {
                    var test = ObjectStateHelper.ConvertObjectState(objectWithState.ObjectState);
                    entry.State = test;
                }
            }
        }

        private static void HandlePartialUpdate(DbContext context, IObjectWithState objectWithState,
            EntityEntry<IObjectWithState> entry)
        {
            var objectWithPartialUpdate = objectWithState as IObjectWithPartialUpdate;

            if (objectWithPartialUpdate == null)
                throw new InvalidOperationException(string.Format("Entity {0} must inheritance from" +
                                                                  " IObjectWithPartialUpdate",
                    objectWithState.GetType()));

            // context.Configuration.ValidateOnSaveEnabled = false;

            entry.State = EntityState.Unchanged;
            //foreach (var property in objectWithPartialUpdate.ModifiedProperties)
            //{
            //    var errors = entry.Property(property).GetValidationErrors();
            //    if (errors.Count == 0)
            //    {
            //        entry.Property(property).IsModified = true;
            //    }
            //    else
            //    {
            //        throw new DbEntityValidationException(string.Format("{0} has some errors.", property),
            //            new[] { new DbEntityValidationResult(entry, errors) });
            //    }
            //}
        }
    }
}