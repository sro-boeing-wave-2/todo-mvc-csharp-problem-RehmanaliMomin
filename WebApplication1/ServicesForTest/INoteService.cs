using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.ServicesForTest
{
    interface INoteService
    {
        IEnumerable<Notes> GetNotes();
        IEnumerable<Notes> GetAllPinnedNotes();
        IEnumerable<Notes> GetNotesFromId(int id);
        IEnumerable<Notes> GetAllByTitleName(string TitleName);
        IEnumerable<Notes> GetNotesFromLabel(string label);
        Notes AddNote(Notes note);
        Notes EditNote(int id, Notes note);
        void DeleteNote(int id);


    }
}
