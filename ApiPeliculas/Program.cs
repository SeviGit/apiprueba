using ApiPeliculas.Data;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

//Configuramos la conexión a sql server
builder.Services.AddDbContext<MiDbContext>(opciones => {
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConnexionSql"));
}
);


//Soporte para caché (video 60)
builder.Services.AddResponseCaching();


//Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();


//Soporte para versionmiento (video 65)
var apiVersioningBuilder = builder.Services.AddApiVersioning(opciones => {
    opciones.AssumeDefaultVersionWhenUnspecified = true;
    opciones.DefaultApiVersion = new ApiVersion(1, 0);
    opciones.ReportApiVersions = true;
    opciones.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version")
        // ...?api-version=1.0
        // new HeaderApiVersionReader("X-Version");
        // new MediaTypeApiVersionReader("ver");
        ); 
});

apiVersioningBuilder.AddApiExplorer(
        opciones => {
            opciones.GroupNameFormat = "'v'VVV";
            opciones.SubstituteApiVersionInUrl = true;
        }
);

var key = builder.Configuration.GetValue<string>("ApiSetting:Secreta");


//Agregamos autoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));


//Configuración de la Autenticación (Video 57, explicación min 28)
builder.Services.AddAuthentication
    (
        x => {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(x => {
        x.RequireHttpsMetadata = false; // en producción lo suyo dería en true para que sea httpS
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    }
    );




builder.Services.AddControllers(opciones => {
    //Cache profile. Un cache global y así no tener que ponerlo en todas partes.(VIDEO 61)
    opciones.CacheProfiles.Add("PorDefecto30Segundos", new CacheProfile() { Duration = 30});
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { // (Video 59 min 14)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = 
        "Autenticación JWT usando el esquema Bearer. \r\n\r\n" +
        "Ingresa la palabra 'Bearer' seguido de un espacio y después su token en el campo de abajo. \r\n\r\n " +
        "Ejemplo:\"Bearer token\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"       
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


//Soporte par CORS
//Se pueden habilitar: 1.Un dominio, 2.Multiples dominios,
//3.Cualquier dominio (Tener cuenta seguridad)
//Usamos de ejemplo el dominio: http://localhost:3223, ***se debe cambiar por el correcto
//Se usa (*) para todos los dominios
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build => {
    build.WithOrigins("http://localhost:3223").AllowAnyMethod().AllowAnyHeader();
})
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//soporte para CORS
app.UseCors("PoliticaCors");

//Soporte para Autenticación
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
