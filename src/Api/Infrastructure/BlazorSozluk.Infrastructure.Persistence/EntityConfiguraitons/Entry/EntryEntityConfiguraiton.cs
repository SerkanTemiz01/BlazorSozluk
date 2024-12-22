using BlazorSozluk.Api.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Infrastructure.Persistence.EntityConfiguraitons.Entry;

public class EntryEntityConfiguraiton:BaseEntityConfiguration<Api.Domain.Models.Entry>    
{
    public override void Configure(EntityTypeBuilder<Domain.Models.Entry> builder)
    {
        base.Configure(builder);
        builder.ToTable("entry",BlazorSozlukContext.DEFAULT_SCHEMA);

        builder.HasOne(i => i.CreatedUser)
            .WithMany(i => i.Entries)
            .HasForeignKey(i => i.CreatedById);
    }

}
