﻿using FluentResults;
using Microsoft.AspNetCore.Mvc;
using UsuariosApi.Data.Request;
using UsuariosApi.Services;

namespace UsuariosApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public IActionResult LogaUsuario(LoginRequest request)
        {
            Result resultado = _loginService.LogaUsuario(request);
            if (resultado.IsFailed)
            {
                return Unauthorized(resultado.Errors);
            }

            // Retorna junto do Sucesso informações que obtiveram sucesso (foram enviadas junto do Result)
            return Ok(resultado.Successes);
        }


    }
}
