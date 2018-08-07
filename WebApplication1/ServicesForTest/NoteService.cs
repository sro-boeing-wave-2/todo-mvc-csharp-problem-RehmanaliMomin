using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.ServicesForTest
{
    public class NoteService : INoteService
    {
        private readonly List<Notes> _notes;

        public NoteService()
        {
            //_notes = new List<Notes>()
            //{
            //    new Notes(){Id =1,Title="OOOONE",IsPinned=true,Label={"" } }
            //}
        }



        public Notes AddNote(Notes note)
        {

            throw new NotImplementedException();
        }

        public void DeleteNote(int id)
        {
            throw new NotImplementedException();
        }

        public Notes EditNote(int id, Notes note)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notes> GetAllByTitleName(string TitleName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notes> GetAllPinnedNotes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notes> GetNotes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notes> GetNotesFromId(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notes> GetNotesFromLabel(string label)
        {
            throw new NotImplementedException();
        }
    }
}
