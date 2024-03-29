﻿using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UsuariosApi.Data.Dtos;
using UsuariosApi.Data.Request;
using UsuariosApi.Models;

namespace UsuariosApi.Services
{
    public class CadastroService
    {
        private IMapper _mapper;
        private UserManager<IdentityUser<int>> _userManager;
        private EmailService _emailService;

        public CadastroService(IMapper mapper,
            UserManager<IdentityUser<int>> userManager,
            EmailService emailService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }

        public Result CadastraUsuario(CreateUsuarioDto createDto)
        {
            Usuario usuario = _mapper.Map<Usuario>(createDto);
            IdentityUser<int> usuarioIdentity = _mapper.Map<IdentityUser<int>>(usuario);

            // Retorna um Task, pois é uma tarefa Assíncrona
            Task<IdentityResult> resultadoIdentity = _userManager
                .CreateAsync(usuarioIdentity, createDto.Password);

            if (resultadoIdentity.Result.Succeeded)
            {
                string code = _userManager
                    .GenerateEmailConfirmationTokenAsync(usuarioIdentity).ToString();

                // Para garantir que no Email o HTML escrito não irá quebrar
                var encodedCode = HttpUtility.UrlEncode(code);

                _emailService.EnviarEmail(
                        new[] { usuarioIdentity.Email },
                        "Link de ativação",
                        usuarioIdentity.Id,
                        encodedCode
                    );

                return Result.Ok()
                    .WithSuccess(code);
            }
            return Result.Fail("Falha ao cadastrar usuário");
        }

        public Result AtivaContaUsuario(AtivaContaRequest request)
        {
            var identityUser = _userManager
                .Users
                .FirstOrDefault(
                    usuario => usuario.Id == request.UsuarioId
                );
            var identityResult = _userManager.ConfirmEmailAsync(
                    identityUser,
                    request.CodigoDeAtivacao
                ).Result;

            if (identityResult.Succeeded)
            {
                return Result.Ok();
            }
            return Result.Fail("Falha ao ativar a conta do usuário");
        }
    }
}
