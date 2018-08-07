using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NotesAPIContext _notesContext;
        
        public NotesController(NotesAPIContext aPIContext)
        {
            _notesContext = aPIContext;
        }

        [HttpGet]
        public IActionResult GetNotes()
        {
            return Ok(_notesContext.notes.Include(x => x.Label).Include(x => x.Content).ToList());

        }

        [HttpGet]
        public async Task<IActionResult> GetAllPinnedNotes()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var n = await _notesContext.notes.Include(x => x.Content).Include(x => x.Label).Where(x => x.IsPinned == true).ToListAsync();
            if (n == null)
            {
                return NotFound();
            }
            return Ok(n);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotesFromId(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var note = await _notesContext.notes.Include(y => y.Label).Include(y => y.Content).SingleOrDefaultAsync(x => x.Id == id);
            if (note == null)
            {
                return NotFound();
            }
            return Ok(note);
        }


        [HttpGet("{TitleName}")]
        public async Task<IActionResult> GetAllByTitleName(string TitleName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            var n = await _notesContext.notes.Include(y => y.Label).Include(y => y.Content).SingleOrDefaultAsync(x => x.Title == TitleName);
            if (n == null)
            {
                return NotFound();
            }
            return Ok(n);
        }


        [HttpGet("{label}")]
        public async Task<IActionResult> GetNotesFromLabel(string label)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var NonNullDatas = _notesContext.notes.Include(s => s.Content).Include(s => s.Label).Where(x => x.Label != null);
            return Ok(await NonNullDatas.Where(x => x.Label.Any(y => y.LabelName == label)).ToListAsync());
            
        }


        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] Notes note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _notesContext.notes.Add(note);
            await _notesContext.SaveChangesAsync();
            return CreatedAtAction("GetNotes", new { id = note.Id }, note);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditNote(long id,[FromBody] Notes note)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Invalid");
                return BadRequest(ModelState);
            }
           

            _notesContext.notes.Update(note);
            await _notesContext.SaveChangesAsync();
            
            return Ok(note);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notes = GetNotes();
            ObjectResult res = notes as OkObjectResult;
            var note = res.Value as List<Notes>;
            Notes n = null;
            foreach (var item in note)
            {
                if (item.Id == id)
                {
                    n = item;
                    break;
                }
            }

            if (n == null)
            {
                return NotFound();
            }
            _notesContext.notes.Remove(n);
            await _notesContext.SaveChangesAsync();

            return NoContent();
        }
    }
}