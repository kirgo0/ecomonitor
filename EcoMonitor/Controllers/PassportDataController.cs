﻿using AutoMapper;
using EcoMonitor.Model;
using EcoMonitor.Model.APIResponses;
using EcoMonitor.Model.DTO.Passport;
using EcoMonitor.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;

namespace EcoMonitor.Controllers
{
    [ApiController]
    [Route("api/PassportData")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class PassportDataController : BasicCRUDController<IPassportRepository, Passport, PassportDTO, PassportCreateDTO, PassportUpdateDTO>
    {
        private readonly ICompanyRepository _repositoryComapny;

        public PassportDataController(ICompanyRepository repositoryComapny, IPassportRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repositoryComapny = repositoryComapny;
        }

        [AllowAnonymous]
        public override Task<ActionResult<APIResponse>> GetAll()
        {
            return base.GetAll();
        }

        [AllowAnonymous]
        public override Task<ActionResult<APIResponse>> Get(int id)
        {
            return base.Get(id);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<APIResponse>> Create([FromBody] PassportCreateDTO createDTO)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                foreach (var modelError in ModelState.Values)
                {
                    foreach (ModelError error in modelError.Errors)
                    {
                        _response.ErrorMessages.Add(error.ErrorMessage);
                    }
                }
                return BadRequest(_response);
            }
            try
            {
                if(await _repositoryComapny.GetAsync(c => c.id == createDTO.company_id) != null)
                {
                    Passport passport = _mapper.Map<Passport>(createDTO);

                    await _repository.CreateAsync(passport);
                    _response.Result = _mapper.Map<PassportDTO>(passport);
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetPassport", new { id = passport.id }, _response);
                } else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("No company with this id was found!");
                    return NotFound(_response);
                }

            }
            catch (DbUpdateException ex)
            {
                MySqlException innerException = ex.InnerException as MySqlException;
                if (innerException != null && (innerException.Number == 1062))
                {
                    _response.StatusCode = HttpStatusCode.Conflict;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Passport with this year already exists");
                    return Conflict(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return StatusCode(500, _response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult<APIResponse>> Update([FromBody] PassportUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                foreach (var modelError in ModelState.Values)
                {
                    foreach (ModelError error in modelError.Errors)
                    {
                        _response.ErrorMessages.Add(error.ErrorMessage);
                    }
                }
                return BadRequest(_response);
            }
            try
            {
                Passport model = _mapper.Map<Passport>(updateDTO);

                if (await _repository.GetAsync(c => c.id == model.id, false) == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }

                if (await _repositoryComapny.GetAsync(c => c.id == updateDTO.company_id) != null)
                {
                    await _repository.UpdateAsync(model);
                    return NoContent();
                }
                else
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("No company with this id was found!");
                    return NotFound(_response);
                }

            }
            catch (DbUpdateException ex)
            {
                MySqlException innerException = ex.InnerException as MySqlException;
                if (innerException != null && (innerException.Number == 1062))
                {
                    _response.StatusCode = HttpStatusCode.Conflict;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Passport with this year already exists");
                    return Conflict(_response);
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return StatusCode(500, _response);
        }
    }
}
