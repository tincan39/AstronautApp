using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("AstronautDetail")]
    public class AstronautDetail
    {
        public static readonly string RETIRED = "RETIRED";
        public int Id { get; set; }

        public int PersonId { get; set; }

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        public virtual Person Person { get; set; }

        public void UpdateAstronautDetail(AstronautDuty duty)
        {
            PersonId = duty.PersonId;
            CurrentRank = duty.Rank;
            CurrentDutyTitle = duty.DutyTitle;
            if (duty.DutyTitle == RETIRED)
            {
                CareerEndDate = duty.DutyStartDate.AddDays(-1);
            }
        }

        public static AstronautDetail CreateAstronautDetail(AstronautDuty duty)
        {
            var detail = new AstronautDetail
            {
                CareerStartDate = duty.DutyStartDate,
                PersonId = duty.PersonId
            };

            detail.UpdateAstronautDetail(duty);
            return detail;
        }

    }

    public class AstronautDetailConfiguration : IEntityTypeConfiguration<AstronautDetail>
    {
        public void Configure(EntityTypeBuilder<AstronautDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
