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
        public IEnumerable<Notes> GetNotes()
        {
            return _notesContext.notes.Include(x => x.Label).Include(x => x.Content);       
        }


        [HttpGet("{TitleName}")]
        public ActionResult<List<Notes>> GetAllByTitleName(string TitleName)
        {
            List<Notes> list = new List<Notes>();

            foreach (Notes item in _notesContext.notes)
            {
                if (item.Title == TitleName) list.Add(item);
            }
            return list;
        }


        [HttpPost]
        public IActionResult AddNote([FromBody] Notes note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _notesContext.Add(note);
            return CreatedAtAction("GetNotes", new { id = note.Id }, note);

        }



      



    }
}