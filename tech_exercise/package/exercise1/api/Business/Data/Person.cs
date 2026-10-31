using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("Person")]
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public virtual AstronautDetail? AstronautDetail { get; set; }

        public virtual ICollection<AstronautDuty> AstronautDuties { get; set; } = new HashSet<AstronautDuty>();

        public void CreateOrUpdateAstronautDetails(AstronautDuty duty)
        {
            if(AstronautDetail == null)
            {
                AstronautDetail = new AstronautDetail();
                AstronautDetail.CareerStartDate = duty.DutyStartDate;
                AstronautDetail.PersonId = duty.PersonId;
            }

            string RETIRED = "RETIRED";

            AstronautDetail.CurrentRank = duty.Rank;
            AstronautDetail.CurrentDutyTitle = duty.DutyTitle;
            if (duty.DutyTitle == RETIRED)
            {
                AstronautDetail.CareerEndDate = duty.DutyStartDate.AddDays(-1);
            }
        }

    }

    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasOne(z => z.AstronautDetail).WithOne(z => z.Person).HasForeignKey<AstronautDetail>(z => z.PersonId);
            builder.HasMany(z => z.AstronautDuties).WithOne(z => z.Person).HasForeignKey(z => z.PersonId);
        }
    }
}
