using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WebApplication1.Controllers;
using WebApplication1.Data;
using WebApplication1.Models;
using Xunit;

namespace XUnitTestProject1
{
    public class BackendTest
    {
        private readonly NotesController _controller;
        

        public BackendTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<NotesAPIContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            NotesAPIContext context = new NotesAPIContext(optionsBuilder.Options);
            _controller = new NotesController(context);
            CreateTestData(context);
        }

        private void CreateTestData(NotesAPIContext context)
        {
            var notes = new List<Notes>();

            Notes note = new Notes();
            note.Title = "title1";
            note.Label = new List<Label>
            {
                new Label{LabelName="label1"},
                new Label{LabelName="label2"}
            };
            note.IsPlainText = true;
            note.IsPinned = true;
            note.Content = new List<Content>()
            {
                   new Content{ ContentData = "content ...... .... ..." }
            };
            notes.Add(note);

            Notes note1 = new Notes();
            note1.Title = "title2";
            note1.Label = new List<Label>
            {
                new Label{LabelName="label21"},
                new Label{LabelName="label2"}
            };
            note1.IsPlainText = false;

            notes.Add(note1);


            Notes note2 = new Notes();
            note2.Title = "title3";
            note2.Label = new List<Label>
            {
                new Label{LabelName="label31"},
                new Label{LabelName="label32"}
            };
            note2.IsPlainText = false;
            note2.IsPinned = true;
            notes.Add(note2);

            //{
            //        Title = "title1",
            //        Label = new List<Label>
            //        {
            //            new Label{LabelName="label1"},
            //            new Label{LabelName="label2"}
            //        },
            //        IsPlainText = true
            //    },

            //    {
            //        Title = "title2",
            //        Label = new List<Label>
            //        {
            //            new Label{LabelName="label21"},
            //            new Label{LabelName="label22"}
            //        },
            //        IsPlainText = false
            //    }
            //};

            List<Notes> temp = notes as List<Notes>;
            
            context.AddRange(notes);
            context.SaveChanges();

            Console.WriteLine(notes[0].Id +" ========= "+notes[1].Id);
        }



        [Fact]
        public void GetNoteTest()
        {
            var result = _controller.GetNotes();
            var objectResult = result as ObjectResult;
            var notes = objectResult.Value as List<Notes>;
            Console.WriteLine(notes[1].Id);
            Assert.Equal(3, notes.Count);
        }

        [Fact]
        public async void EditTest()
        {

            Notes note = new Notes();
            note.Title = "title1-edited";
            note.Label = new List<Label>
            {
                new Label{LabelName="label1"},
                new Label{LabelName="label2"},
               // new Label{LabelName="label2 edited"}

            };
            note.IsPlainText = false;
            note.IsPinned = false;
            note.Content = new List<Content>()
            {
                   new Content{ ContentData = "content ...... .... ..." },
                   //new Content{ ContentData = "content edited ...... .... ..." }
            };

            // Console.WriteLine("----------");
            var result1 = _controller.GetNotes();
            var objectResult = result1 as ObjectResult;
            var notes = objectResult.Value as List<Notes>;
            //Console.WriteLine("Count = "+notes.Count);
            var id = notes[0].Id;

            //Console.WriteLine(notes[0].Id);
            var result = await _controller.EditNote(id, note);
            var responseOkObject = result as OkObjectResult;
            var n = responseOkObject.Value as Notes;
            //Console.WriteLine(n.Title);
            //Console.WriteLine("===========");
            Assert.Equal(note.Title, n.Title);

        }



        [Fact]
        public async void TestGetId()
        {
            var result1 = _controller.GetNotes();
            var objectResult = result1 as ObjectResult;
            var note = objectResult.Value as List<Notes>;
            //Console.WriteLine("Count = "+notes.Count);
            var id = note[0].Id;
            var result = await _controller.GetNotesFromId(id);
            //Assert.True(condition: result is OkObjectResult);
            var OkObjectResult = result as OkObjectResult;
            //Assert.True(condition: result, OkObjectResult);
            var notes = OkObjectResult.Value as Notes;
            Console.WriteLine(notes.Title);
            Assert.Equal(id, notes.Id);
            Assert.Equal(200, OkObjectResult.StatusCode);
        }



        [Fact]
        public async void TestGetTitle()
        {
            string s = "title2";
            var result = await _controller.GetAllByTitleName(s);
            var resultAsOkObjectResult = result as OkObjectResult;
            //Assert.True(condition: result, OkObjectResult);
            var notes = resultAsOkObjectResult.Value as Notes;

            Assert.Equal("title2", notes.Title);
        }

        [Fact]
        public async void TestGetAllPinnedNotes()
        {
            var result = await _controller.GetAllPinnedNotes();
            var resultAsOkObjectResult = result as OkObjectResult;
            //Assert.True(condition: result, OkObjectResult);
            var notes = resultAsOkObjectResult.Value as List<Notes>;
            bool flag = true;
            foreach (var item in notes)
            {
                if (!item.IsPinned) flag = false;
            }
            Assert.True(flag);
        }


        [Fact]
        public async void TestGetAllNotesFromLabel()
        {
            string s = "label2";
            var result = await _controller.GetNotesFromLabel(s);
            var resultAsOkObjectResult = result as OkObjectResult;
            //Assert.True(condition: result, OkObjectResult);
            var notes = resultAsOkObjectResult.Value as List<Notes>;
            bool flag1 = true;
            Console.WriteLine(notes.Count);
            foreach (var item in notes)
            {
                bool flag = false;
                foreach (var lab in item.Label)
                {
                    if (lab.LabelName == s)
                    {
                        flag = true;
                    }
                }
                if (!flag) flag1 = false;
            }
            Assert.True(flag1);
        }

        [Fact]
        public async void PostTest()
        {

            Notes note = new Notes();
            note.Title = "title4";
            note.Label = new List<Label>
            {
                new Label{LabelName="label4"},
                new Label{LabelName="label42"}
            };
            note.IsPlainText = true;
            note.IsPinned = true;
            note.Content = new List<Content>()
            {
                   new Content{ ContentData = " new, content ...... .... ..." }
            };

            var result = await _controller.AddNote(note);
            var OkObjectResult = result as CreatedAtActionResult;
            var n = OkObjectResult.Value as Notes;
            Assert.Equal(note.Title, n.Title);
            Assert.Equal(201, OkObjectResult.StatusCode);
        }

        [Fact]
        public async void DeleteTest()
        {
            var result1 = _controller.GetNotes();
            var objectResult = result1 as ObjectResult;
            var note = objectResult.Value as List<Notes>;
            //Console.WriteLine("Count = "+notes.Count);
            var id = note[0].Id;
            var result = await _controller.DeleteNote(id);

            bool flag = true;

            var result2 = _controller.GetNotes();
            var objectResult2 = result2 as ObjectResult;
            var note2 = objectResult2.Value as List<Notes>;
            
            foreach (var item in note2)
            {
                if(item.Id==id)
                {
                    flag = false;
                    break;
                }
            }

            Assert.True(flag);
        }



    }
}
