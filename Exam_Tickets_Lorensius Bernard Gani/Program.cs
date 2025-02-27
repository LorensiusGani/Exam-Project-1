using Exam.Entities;
using Exam_Tickets_Lorensius_Bernard_Gani.Services;
using Exam_Tickets_Lorensius_Bernard_Gani.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<AccelokaContext>(options =>
{
    var constring = configuration.GetConnectionString("SQLServerDB");
    options.UseSqlServer(constring);
});

builder.Services.AddHttpContextAccessor();


//MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssemblyContaining<TicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PostBookedValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteBookedValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateValidator>();


builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddTransient<TicketsServices>();
builder.Services.AddTransient<BookedTicketServices>();

var app = builder.Build();
app.UseCors("AllowAll");

app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is ProblemDetailsException problemDetailsException)
        {
            context.Response.StatusCode = problemDetailsException.ProblemDetails.Status ?? 500;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetailsException.ProblemDetails);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = 500,
                Detail = exception?.Message
            });
        }
    });
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
