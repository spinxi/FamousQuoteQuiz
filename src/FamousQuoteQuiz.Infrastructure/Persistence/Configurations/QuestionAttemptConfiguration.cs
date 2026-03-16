using FamousQuoteQuiz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamousQuoteQuiz.Infrastructure.Persistence.Configurations;

public sealed class QuestionAttemptConfiguration : IEntityTypeConfiguration<QuestionAttempt>
{
    public void Configure(EntityTypeBuilder<QuestionAttempt> builder)
    {
        builder.ToTable("QuestionAttempts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PresentedAuthorCandidate).HasMaxLength(200);
        builder.Property(x => x.UserAnswer).HasMaxLength(200).IsRequired();

        builder.HasOne(x => x.GameSession)
            .WithMany(x => x.QuestionAttempts)
            .HasForeignKey(x => x.GameSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.QuestionAttempts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quote)
            .WithMany(x => x.QuestionAttempts)
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.AnsweredAtUtc });
        builder.HasIndex(x => new { x.IsCorrect, x.AnsweredAtUtc });
        builder.HasIndex(x => x.QuizMode);
    }
}
