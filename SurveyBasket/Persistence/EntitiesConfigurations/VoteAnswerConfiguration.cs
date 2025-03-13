
namespace SurveyBasket.Persistence.EntitiesConfigurations
{
    public class VoteAnswerConfiguration : IEntityTypeConfiguration<VoteAnswer>
    {
        public void Configure(EntityTypeBuilder<VoteAnswer> builder)
        {
            builder.HasIndex(e => new { e.VoteId, e.QuestionId }).IsUnique();
        }
    }
}
