using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RescateEmocional.Models;

public partial class RescateEmocionalContext : DbContext
{
    public RescateEmocionalContext()
    {
    }

    public RescateEmocionalContext(DbContextOptions<RescateEmocionalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Conversacion> Conversacions { get; set; }

    public virtual DbSet<Diario> Diarios { get; set; }

    public virtual DbSet<Etiquetum> Etiqueta { get; set; }

    public virtual DbSet<Mensaje> Mensajes { get; set; }

    public virtual DbSet<Organizacion> Organizacions { get; set; }

    public virtual DbSet<PeticionVerificacion> PeticionVerificacions { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Telefono> Telefonos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.Idadmin).HasName("PK__Administ__D704F3E832EA1F36");

            entity.ToTable("Administrador");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__Administ__531402F320984443").IsUnique();

            entity.Property(e => e.Idadmin).HasColumnName("IDAdmin");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Idrol).HasColumnName("IDRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdrolNavigation).WithMany(p => p.Administradors)
                .HasForeignKey(d => d.Idrol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Administr__IDRol__4CA06362");

            entity.HasMany(d => d.Idorganizacions).WithMany(p => p.Idadmins)
                .UsingEntity<Dictionary<string, object>>(
                    "AdministradorOrganizacion",
                    r => r.HasOne<Organizacion>().WithMany()
                        .HasForeignKey("Idorganizacion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Administr__IDOrg__5EBF139D"),
                    l => l.HasOne<Administrador>().WithMany()
                        .HasForeignKey("Idadmin")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Administr__IDAdm__5DCAEF64"),
                    j =>
                    {
                        j.HasKey("Idadmin", "Idorganizacion").HasName("PK__Administ__6F856700E8FDB013");
                        j.ToTable("AdministradorOrganizacion");
                        j.IndexerProperty<int>("Idadmin").HasColumnName("IDAdmin");
                        j.IndexerProperty<int>("Idorganizacion").HasColumnName("IDOrganizacion");
                    });

            entity.HasMany(d => d.Idusuarios).WithMany(p => p.Idadmins)
                .UsingEntity<Dictionary<string, object>>(
                    "AdministradorUsuario",
                    r => r.HasOne<Usuario>().WithMany()
                        .HasForeignKey("Idusuario")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Administr__IDUsu__5AEE82B9"),
                    l => l.HasOne<Administrador>().WithMany()
                        .HasForeignKey("Idadmin")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Administr__IDAdm__59FA5E80"),
                    j =>
                    {
                        j.HasKey("Idadmin", "Idusuario").HasName("PK__Administ__5227E2FE10A09599");
                        j.ToTable("AdministradorUsuario");
                        j.IndexerProperty<int>("Idadmin").HasColumnName("IDAdmin");
                        j.IndexerProperty<int>("Idusuario").HasColumnName("IDUsuario");
                    });
        });

        modelBuilder.Entity<Conversacion>(entity =>
        {
            entity.HasKey(e => e.Idconversacion).HasName("PK__Conversa__ED936DBBB0C2A385");

            entity.ToTable("Conversacion");

            entity.Property(e => e.Idconversacion).HasColumnName("IDConversacion");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.Idorganizacion).HasColumnName("IDOrganizacion");
            entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");
            entity.Property(e => e.Mensaje).HasColumnType("text");

            entity.HasOne(d => d.IdorganizacionNavigation).WithMany(p => p.Conversacions)
                .HasForeignKey(d => d.Idorganizacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conversac__IDOrg__5441852A");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Conversacions)
                .HasForeignKey(d => d.Idusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conversac__IDUsu__534D60F1");
        });

        modelBuilder.Entity<Diario>(entity =>
        {
            entity.HasKey(e => e.Iddiario).HasName("PK__Diario__5F752C6285678152");

            entity.ToTable("Diario");

            entity.Property(e => e.Iddiario).HasColumnName("IDDiario");
            entity.Property(e => e.Contenido).HasColumnType("text");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");
            entity.Property(e => e.Titulo)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Diarios)
                .HasForeignKey(d => d.Idusuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Diario__IDUsuari__619B8048");
        });

        modelBuilder.Entity<Etiquetum>(entity =>
        {
            entity.HasKey(e => e.Idetiqueta).HasName("PK__Etiqueta__E919DC2E2FEC45BB");

            entity.Property(e => e.Idetiqueta).HasColumnName("IDEtiqueta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.HasKey(e => e.Idmensaje).HasName("PK__Mensaje__104235383A4B3916");

            entity.ToTable("Mensaje");

            entity.Property(e => e.Idmensaje).HasColumnName("IDMensaje");
            entity.Property(e => e.Contenido).HasColumnType("text");
            entity.Property(e => e.FechaHora).HasColumnType("datetime");
            entity.Property(e => e.Idconversacion).HasColumnName("IDConversacion");

            entity.HasOne(d => d.IdconversacionNavigation).WithMany(p => p.Mensajes)
                .HasForeignKey(d => d.Idconversacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mensaje__IDConve__571DF1D5");
        });

        modelBuilder.Entity<Organizacion>(entity =>
        {
            entity.HasKey(e => e.Idorganizacion).HasName("PK__Organiza__88194E8A26C39EC4");

            entity.ToTable("Organizacion");

            entity.Property(e => e.Idorganizacion).HasColumnName("IDOrganizacion");
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Horario)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Idrol).HasColumnName("IDRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdrolNavigation).WithMany(p => p.Organizacions)
                .HasForeignKey(d => d.Idrol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Organizac__IDRol__3D5E1FD2");

            entity.HasMany(d => d.Idetiqueta).WithMany(p => p.Idorganizacions)
                .UsingEntity<Dictionary<string, object>>(
                    "OrganizacionEtiquetum",
                    r => r.HasOne<Etiquetum>().WithMany()
                        .HasForeignKey("Idetiqueta")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Organizac__IDEti__48CFD27E"),
                    l => l.HasOne<Organizacion>().WithMany()
                        .HasForeignKey("Idorganizacion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Organizac__IDOrg__47DBAE45"),
                    j =>
                    {
                        j.HasKey("Idorganizacion", "Idetiqueta").HasName("PK__Organiza__7688D34837D05AD2");
                        j.ToTable("OrganizacionEtiqueta");
                        j.IndexerProperty<int>("Idorganizacion").HasColumnName("IDOrganizacion");
                        j.IndexerProperty<int>("Idetiqueta").HasColumnName("IDEtiqueta");
                    });

            entity.HasMany(d => d.Idtelefonos).WithMany(p => p.Idorganizacions)
                .UsingEntity<Dictionary<string, object>>(
                    "OrganizacionTelefono",
                    r => r.HasOne<Telefono>().WithMany()
                        .HasForeignKey("Idtelefono")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Organizac__IDTel__4316F928"),
                    l => l.HasOne<Organizacion>().WithMany()
                        .HasForeignKey("Idorganizacion")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Organizac__IDOrg__4222D4EF"),
                    j =>
                    {
                        j.HasKey("Idorganizacion", "Idtelefono").HasName("PK__Organiza__2DA0F3682484643B");
                        j.ToTable("OrganizacionTelefono");
                        j.IndexerProperty<int>("Idorganizacion").HasColumnName("IDOrganizacion");
                        j.IndexerProperty<int>("Idtelefono").HasColumnName("IDTelefono");
                    });
        });

        modelBuilder.Entity<PeticionVerificacion>(entity =>
        {
            entity.HasKey(e => e.Idpeticion).HasName("PK__Peticion__1A9EBE04ECE4F7F9");

            entity.ToTable("PeticionVerificacion");

            entity.Property(e => e.Idpeticion).HasColumnName("IDPeticion");
            entity.Property(e => e.Comentarios).HasColumnType("text");
            entity.Property(e => e.FechaSolicitud).HasColumnType("datetime");
            entity.Property(e => e.Idadmin).HasColumnName("IDAdmin");
            entity.Property(e => e.Idorganizacion).HasColumnName("IDOrganizacion");

            entity.HasOne(d => d.IdadminNavigation).WithMany(p => p.PeticionVerificacions)
                .HasForeignKey(d => d.Idadmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PeticionV__IDAdm__5070F446");

            entity.HasOne(d => d.IdorganizacionNavigation).WithMany(p => p.PeticionVerificacions)
                .HasForeignKey(d => d.Idorganizacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PeticionV__IDOrg__4F7CD00D");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Idrol).HasName("PK__Rol__A681ACB68E85EFA2");

            entity.ToTable("Rol");

            entity.Property(e => e.Idrol).HasColumnName("IDRol");
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Telefono>(entity =>
        {
            entity.HasKey(e => e.Idtelefono).HasName("PK__Telefono__5B9BDE2BE5CB0B38");

            entity.ToTable("Telefono");

            entity.Property(e => e.Idtelefono).HasColumnName("IDTelefono");
            entity.Property(e => e.TipoDeNumero)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Idusuario).HasName("PK__Usuario__523111694A0F809D");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__Usuario__531402F3DDBF84F2").IsUnique();

            entity.Property(e => e.Idusuario).HasColumnName("IDUsuario");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Idrol).HasColumnName("IDRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdrolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Idrol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuario__IDRol__3A81B327");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
