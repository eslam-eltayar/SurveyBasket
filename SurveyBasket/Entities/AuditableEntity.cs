﻿namespace SurveyBasket.Entities
{
    public class AuditableEntity
    {
        public string CreatedById { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedById { get; set; } 


        public ApplicationUser CreatedBy { get; set; } = default!;
        public ApplicationUser? UpdatedBy { get; set; } 
    }
}
