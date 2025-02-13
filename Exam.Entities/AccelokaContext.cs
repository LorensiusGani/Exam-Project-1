using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Exam.Entities;

public partial class AccelokaContext : DbContext
{
    public AccelokaContext()
    {
    }

    public AccelokaContext(DbContextOptions<AccelokaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookTicket> BookTickets { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookTicket>(entity =>
        {
            entity.Property(e => e.BookTicketId).HasColumnName("BookTicketID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.TicketName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.TicketCodeNavigation).WithMany(p => p.BookTickets)
                .HasForeignKey(d => d.TicketCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookTickets_Tickets");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketCode);

            entity.Property(e => e.TicketCode)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EventDate).HasColumnType("datetime");
            entity.Property(e => e.TicketName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
