using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RescateEmocional.Migrations
{
    /// <inheritdoc />
    public partial class AddSenderTypeToConversacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Etiqueta",
                columns: table => new
                {
                    IDEtiqueta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Etiqueta__E919DC2E2FEC45BB", x => x.IDEtiqueta);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    IDRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol__A681ACB68E85EFA2", x => x.IDRol);
                });

            migrationBuilder.CreateTable(
                name: "Telefono",
                columns: table => new
                {
                    IDTelefono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDeNumero = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Telefono__5B9BDE2BE5CB0B38", x => x.IDTelefono);
                });

            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    IDAdmin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Contrasena = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    IDRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__D704F3E832EA1F36", x => x.IDAdmin);
                    table.ForeignKey(
                        name: "FK__Administr__IDRol__4CA06362",
                        column: x => x.IDRol,
                        principalTable: "Rol",
                        principalColumn: "IDRol");
                });

            migrationBuilder.CreateTable(
                name: "Organizacion",
                columns: table => new
                {
                    IDOrganizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Horario = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Ubicacion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false),
                    IDRol = table.Column<int>(type: "int", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Organiza__88194E8A26C39EC4", x => x.IDOrganizacion);
                    table.ForeignKey(
                        name: "FK__Organizac__IDRol__3D5E1FD2",
                        column: x => x.IDRol,
                        principalTable: "Rol",
                        principalColumn: "IDRol");
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IDUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Contrasena = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false),
                    IDRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__523111694A0F809D", x => x.IDUsuario);
                    table.ForeignKey(
                        name: "FK__Usuario__IDRol__3A81B327",
                        column: x => x.IDRol,
                        principalTable: "Rol",
                        principalColumn: "IDRol");
                });

            migrationBuilder.CreateTable(
                name: "AdministradorOrganizacion",
                columns: table => new
                {
                    IDAdmin = table.Column<int>(type: "int", nullable: false),
                    IDOrganizacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__6F856700E8FDB013", x => new { x.IDAdmin, x.IDOrganizacion });
                    table.ForeignKey(
                        name: "FK__Administr__IDAdm__5DCAEF64",
                        column: x => x.IDAdmin,
                        principalTable: "Administrador",
                        principalColumn: "IDAdmin");
                    table.ForeignKey(
                        name: "FK__Administr__IDOrg__5EBF139D",
                        column: x => x.IDOrganizacion,
                        principalTable: "Organizacion",
                        principalColumn: "IDOrganizacion");
                });

            migrationBuilder.CreateTable(
                name: "OrganizacionEtiqueta",
                columns: table => new
                {
                    IDOrganizacion = table.Column<int>(type: "int", nullable: false),
                    IDEtiqueta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Organiza__7688D34837D05AD2", x => new { x.IDOrganizacion, x.IDEtiqueta });
                    table.ForeignKey(
                        name: "FK__Organizac__IDEti__48CFD27E",
                        column: x => x.IDEtiqueta,
                        principalTable: "Etiqueta",
                        principalColumn: "IDEtiqueta");
                    table.ForeignKey(
                        name: "FK__Organizac__IDOrg__47DBAE45",
                        column: x => x.IDOrganizacion,
                        principalTable: "Organizacion",
                        principalColumn: "IDOrganizacion");
                });

            migrationBuilder.CreateTable(
                name: "OrganizacionTelefono",
                columns: table => new
                {
                    IDOrganizacion = table.Column<int>(type: "int", nullable: false),
                    IDTelefono = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Organiza__2DA0F3682484643B", x => new { x.IDOrganizacion, x.IDTelefono });
                    table.ForeignKey(
                        name: "FK__Organizac__IDOrg__4222D4EF",
                        column: x => x.IDOrganizacion,
                        principalTable: "Organizacion",
                        principalColumn: "IDOrganizacion");
                    table.ForeignKey(
                        name: "FK__Organizac__IDTel__4316F928",
                        column: x => x.IDTelefono,
                        principalTable: "Telefono",
                        principalColumn: "IDTelefono");
                });

            migrationBuilder.CreateTable(
                name: "PeticionVerificacion",
                columns: table => new
                {
                    IDPeticion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDOrganizacion = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime", nullable: false),
                    Comentarios = table.Column<string>(type: "text", nullable: true),
                    IDAdmin = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Peticion__1A9EBE04ECE4F7F9", x => x.IDPeticion);
                    table.ForeignKey(
                        name: "FK__PeticionV__IDAdm__5070F446",
                        column: x => x.IDAdmin,
                        principalTable: "Administrador",
                        principalColumn: "IDAdmin");
                    table.ForeignKey(
                        name: "FK__PeticionV__IDOrg__4F7CD00D",
                        column: x => x.IDOrganizacion,
                        principalTable: "Organizacion",
                        principalColumn: "IDOrganizacion");
                });

            migrationBuilder.CreateTable(
                name: "AdministradorUsuario",
                columns: table => new
                {
                    IDAdmin = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__5227E2FE10A09599", x => new { x.IDAdmin, x.IDUsuario });
                    table.ForeignKey(
                        name: "FK__Administr__IDAdm__59FA5E80",
                        column: x => x.IDAdmin,
                        principalTable: "Administrador",
                        principalColumn: "IDAdmin");
                    table.ForeignKey(
                        name: "FK__Administr__IDUsu__5AEE82B9",
                        column: x => x.IDUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IDUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Conversacion",
                columns: table => new
                {
                    IDConversacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<int>(type: "int", nullable: true),
                    IDOrganizacion = table.Column<int>(type: "int", nullable: true),
                    SenderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    Mensaje = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Conversa__ED936DBBB0C2A385", x => x.IDConversacion);
                    table.ForeignKey(
                        name: "FK__Conversac__IDOrg__5441852A",
                        column: x => x.IDOrganizacion,
                        principalTable: "Organizacion",
                        principalColumn: "IDOrganizacion");
                    table.ForeignKey(
                        name: "FK__Conversac__IDUsu__534D60F1",
                        column: x => x.IDUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IDUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Diario",
                columns: table => new
                {
                    IDDiario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Diario__5F752C6285678152", x => x.IDDiario);
                    table.ForeignKey(
                        name: "FK__Diario__IDUsuari__619B8048",
                        column: x => x.IDUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IDUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Mensaje",
                columns: table => new
                {
                    IDMensaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDConversacion = table.Column<int>(type: "int", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime", nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mensaje__104235383A4B3916", x => x.IDMensaje);
                    table.ForeignKey(
                        name: "FK__Mensaje__IDConve__571DF1D5",
                        column: x => x.IDConversacion,
                        principalTable: "Conversacion",
                        principalColumn: "IDConversacion");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrador_IDRol",
                table: "Administrador",
                column: "IDRol");

            migrationBuilder.CreateIndex(
                name: "UQ__Administ__531402F320984443",
                table: "Administrador",
                column: "CorreoElectronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdministradorOrganizacion_IDOrganizacion",
                table: "AdministradorOrganizacion",
                column: "IDOrganizacion");

            migrationBuilder.CreateIndex(
                name: "IX_AdministradorUsuario_IDUsuario",
                table: "AdministradorUsuario",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Conversacion_IDOrganizacion",
                table: "Conversacion",
                column: "IDOrganizacion");

            migrationBuilder.CreateIndex(
                name: "IX_Conversacion_IDUsuario",
                table: "Conversacion",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Diario_IDUsuario",
                table: "Diario",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Mensaje_IDConversacion",
                table: "Mensaje",
                column: "IDConversacion");

            migrationBuilder.CreateIndex(
                name: "IX_Organizacion_IDRol",
                table: "Organizacion",
                column: "IDRol");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizacionEtiqueta_IDEtiqueta",
                table: "OrganizacionEtiqueta",
                column: "IDEtiqueta");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizacionTelefono_IDTelefono",
                table: "OrganizacionTelefono",
                column: "IDTelefono");

            migrationBuilder.CreateIndex(
                name: "IX_PeticionVerificacion_IDAdmin",
                table: "PeticionVerificacion",
                column: "IDAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_PeticionVerificacion_IDOrganizacion",
                table: "PeticionVerificacion",
                column: "IDOrganizacion");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IDRol",
                table: "Usuario",
                column: "IDRol");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuario__531402F3DDBF84F2",
                table: "Usuario",
                column: "CorreoElectronico",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdministradorOrganizacion");

            migrationBuilder.DropTable(
                name: "AdministradorUsuario");

            migrationBuilder.DropTable(
                name: "Diario");

            migrationBuilder.DropTable(
                name: "Mensaje");

            migrationBuilder.DropTable(
                name: "OrganizacionEtiqueta");

            migrationBuilder.DropTable(
                name: "OrganizacionTelefono");

            migrationBuilder.DropTable(
                name: "PeticionVerificacion");

            migrationBuilder.DropTable(
                name: "Conversacion");

            migrationBuilder.DropTable(
                name: "Etiqueta");

            migrationBuilder.DropTable(
                name: "Telefono");

            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "Organizacion");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
