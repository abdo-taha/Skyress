using MediatR;
using Microsoft.AspNetCore.Authorization;
using Skyress.Application.Auth.Commands.Login;
using Skyress.Application.Auth.Commands.Register;
using Skyress.Application.Auth.Commands.RefreshToken;
using Skyress.Application.Auth.Commands.Logout;
using Skyress.DTOs.Auth;

namespace Skyress.Endpoints.Auth;

public static class AuthApiRegistration
{
	public static void MapAuthApi(this WebApplication app)
	{
		var group = app.MapGroup("/api/v1/auth")
			.WithTags("Auth");

		group.MapPost("/register", Register)
			.WithName("Register")
			.WithOpenApi()
			.AllowAnonymous();

		group.MapPost("/login", Login)
			.WithName("Login")
			.WithOpenApi()
			.AllowAnonymous();

		group.MapPost("/refresh", Refresh)
			.WithName("Refresh")
			.WithOpenApi()
			.AllowAnonymous();

		group.MapPost("/logout", Logout)
			.WithName("Logout")
			.WithOpenApi()
			.AllowAnonymous();
	}

	private static async Task<IResult> Register(
		RegisterRequest request,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var command = new RegisterCommand(request.Email, request.Password);
			var result = await mediator.Send(command, cancellationToken);
			return Results.Created("/api/v1/auth/register", result.Value);
		}
		catch (InvalidOperationException)
		{
			return Results.StatusCode(StatusCodes.Status409Conflict);
		}
		catch (Exception)
		{
			return Results.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	private static async Task<IResult> Login(
		LoginRequest request,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var command = new LoginCommand(request.Email, request.Password);
			var result = await mediator.Send(command, cancellationToken);
			return Results.Ok(result.Value);
		}
		catch (UnauthorizedAccessException)
		{
			return Results.Unauthorized();
		}
		catch (InvalidOperationException)
		{
			return Results.StatusCode(StatusCodes.Status403Forbidden);
		}
		catch (Exception)
		{
			return Results.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	private static async Task<IResult> Refresh(
		RefreshTokenRequest request,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var command = new RefreshTokenCommand(request.RefreshToken);
			var result = await mediator.Send(command, cancellationToken);
			return Results.Ok(result.Value);
		}
		catch (UnauthorizedAccessException)
		{
			return Results.Unauthorized();
		}
		catch (Exception)
		{
			return Results.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}

	private static async Task<IResult> Logout(
		LogoutRequest request,
		IMediator mediator,
		CancellationToken cancellationToken)
	{
		try
		{
			var command = new LogoutCommand(request.RefreshToken);
			await mediator.Send(command, cancellationToken);
			return Results.NoContent();
		}
		catch (Exception)
		{
			return Results.StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
