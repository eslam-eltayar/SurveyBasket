
namespace SurveyBasket.Persistence.EntitiesConfigurations
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.HasIndex(e => new { e.PollId, e.UserId }).IsUnique();
        }
    }
}
