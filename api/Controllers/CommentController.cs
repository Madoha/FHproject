using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using api.Models;
using api.Extensions;
using Microsoft.Extensions.FileProviders;
using api.Helpers;
using Microsoft.AspNetCore.Authorization;
using api.Helpers;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPService _fmpService;

        public CommentController(ICommentRepository commentRepository, 
        IStockRepository stockRepository, UserManager<AppUser> userManager,
        IFMPService fmpService)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _userManager = userManager;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromRoute] CommentQueryObject queryObject){
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var comments = await _commentRepository.GetAllAsync(queryObject);
            var commentDto = comments.Select(c => c.ToCommentDto());
            return Ok(commentDto);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id){
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            
            var comment = await _commentRepository.GetByIdAsync(id);

            if (comment == null){
                return NotFound();
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{symbol:alpha}")]
        public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            
            var stock = await _stockRepository.GetBySymbolAsync(symbol);

            if (stock == null){
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null){
                    return BadRequest("Stock does not exists");
                } else {
                    await _stockRepository.CreateAsync(stock);
                }
            }

            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);

            var commentModel = commentDto.ToCommentFromCreate(stock.Id);
            commentModel.AppUserId = appUser.Id;

            await _commentRepository.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            
            var deleteResult = await _commentRepository.DeleteAsync(id);

            if (deleteResult == null){
                return NotFound("Comment does not exist");
            }

            return Ok(deleteResult);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody] UpdateCommentRequestDto updateDto){
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            
            var comment = await _commentRepository.UpdateAsync(id, updateDto.ToCommentFromUpdate());

            if (comment == null){
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDto());
        }
    }
}