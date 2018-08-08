using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using WebApplication1;
using WebApplication1.Data;
using WebApplication1.Models;
using Xunit;

namespace XUnitTestProject1
{
    public class IntegrationTest
    {

        

        Notes note = new Notes()
        {
            Title = "My-first-note",
            IsPinned = true,
            IsPlainText = false,
            Label = new List<Label>
               {
                   new Label { LabelName = "Work"},
                   new Label { LabelName = "Play"}
               },
            Content = new List<Content>
               {
                   new Content { ContentData = "Pen"},
                   new Content { ContentData = "Bag"}
               }
        };

        Notes note1 = new Notes()
        {
            Title = "My-second-note",
            IsPinned = false,
            IsPlainText = true,
            Label = new List<Label>
               {
                   new Label { LabelName = "Work-2"},
                   new Label { LabelName = "Play"}
               },
            Content = new List<Content>
               {
                   new Content { ContentData = "Pen-2"},
                   new Content { ContentData = "Bag-2"}
               }
        };


        private HttpClient _client;
        private NotesAPIContext _context;
        public List<Notes> notes;
        

        //Notes note;

        public IntegrationTest()
        {
            var host = new TestServer(
                new WebHostBuilder()
                .UseEnvironment("Testing")
                .UseStartup<Startup>());

            _context = host.Host.Services.GetService(typeof(NotesAPIContext)) as NotesAPIContext;
            _client = host.CreateClient();

            notes = new List<Notes>();
            notes.Add(note);
            notes.Add(note1);
            _context.AddRange(notes);
            _context.SaveChanges();
            //CreateList(notes);

        }

        [Fact]
        public async void TestGetResponseCodeRequest()
        {
            var response = await _client.GetAsync("/api/notes/GetNotes");
           // Console.WriteLine(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async void IntegrationTestGetAllNotes()
        {
            //Act
            var response = await _client.GetAsync("/api/Notes/GetNotes");

            //Assert
            response.EnsureSuccessStatusCode();

            //Reads the response string
            var responseString = await response.Content.ReadAsStringAsync();

            //convert into object of list of notes
            var notes = JsonConvert.DeserializeObject<List<Notes>>(responseString);

            notes.Count().Should().Be(2);

            //Console.WriteLine(notes[0].Id);
            //Console.WriteLine(notes[1].Id);

            //Console.WriteLine(notes[0].Title);
            //Console.WriteLine(notes[1].Title);
            // Console.WriteLine("GetAllNotes" + notes.Count);
            //Console.WriteLine(notes);
        }


        [Fact]
        public async void TestGetByIdRequest()
        {
            var response = await _client.GetAsync("/api/notes/GetNotesFromId/1");

            //Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            //convert into object of list of notes
            var notes = JsonConvert.DeserializeObject<Notes>(responseString);

            Assert.Equal(1, notes.Id);
        }


        [Fact]
        public async void TestGetByWrongIdRequest()
        {
            var response = await _client.GetAsync("/api/notes/GetNotesFromId/500");
            

            var responseString = await response.Content.ReadAsStringAsync();
            //convert into object of list of notes
            var notes = JsonConvert.DeserializeObject<Notes>(responseString);

            Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
        }



        [Fact]
        public async void TestGetByTitleRequest()
        {
            var response = await _client.GetAsync("/api/notes/GetAllByTitleName/My-first-note");

            //Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            //convert into object of list of notes
            var notes = JsonConvert.DeserializeObject<Notes>(responseString);

            Assert.Equal("My-first-note", notes.Title);
        }


        [Fact]
        public async void TestGetByLabelRequest()
        {
            string s = "Work";
            var response = await _client.GetAsync("/api/notes/GetNotesFromLabel/Work");

            //Assert
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            //convert into object of list of notes
            var notes = JsonConvert.DeserializeObject<List<Notes>>(responseString);

            bool flag1 = true;
            //Console.WriteLine(notes.Count);
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
            if (notes == null) Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            else
            {
                response.EnsureSuccessStatusCode();
                Assert.True(flag1);
            }
        }

        [Fact]
        public async void TestGetByWrongLabelRequest()
        {
            string s = "Tatti";
            var response = await _client.GetAsync("/api/notes/GetNotesFromLabel/Tatti");
            var responseString = await response.Content.ReadAsStringAsync();
            var notes = JsonConvert.DeserializeObject<List<Notes>>(responseString);

            if (notes == null) Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
            else
            {
                response.EnsureSuccessStatusCode();
                bool flag1 = true;

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
        }


        [Fact]
        public async void TestGetAllPinnedRequest()
        {
            var response = await _client.GetAsync("/api/notes/GetAllPinnedNotes");
            var responseString = await response.Content.ReadAsStringAsync();
            var notes = JsonConvert.DeserializeObject<List<Notes>>(responseString);
            response.EnsureSuccessStatusCode();

            Assert.Single(notes);
            Assert.Equal("My-first-note", notes[0].Title);

        }



        [Fact]
        public async void TestPostRequest()
        {

            var newNote = new Notes
            {
                Title = "titleNew",
                Label = new List<Label>
                    {
                        new Label{ LabelName="label21New" },
                        new Label{ LabelName="label22New" }
                    },
                IsPlainText = false
            };

            _context.Add(newNote);

            var stringContent = new StringContent(JsonConvert.SerializeObject(newNote), UnicodeEncoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/notes/AddNote", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var noteFromPost = JsonConvert.DeserializeObject <Notes>(responseString);
            
            response.EnsureSuccessStatusCode();

            Assert.Equal(newNote.Title,noteFromPost.Title);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }


        [Fact]
        public async void TestPutRequest()
        {

            Notes noteEditedNew = new Notes()
            {
                Id = 1,
                Title = "My-first-note-edited",
                IsPinned = false,
                IsPlainText = true,
               // Label = new List<Label>
               //{
               //    new Label { LabelName = "Work"},
               //    new Label { LabelName = "Play"}
               //},
               // Content = new List<Content>
               //{
               //    new Content { ContentData = "Pen"},
               //    new Content { ContentData = "Bag"}
               //}
            };

            var json = JsonConvert.SerializeObject(noteEditedNew);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Notes/EditNote/1", stringContent);
            
           

            foreach (var item in _context.notes)
            {
                Console.WriteLine(item.Id + " - "+item.Title);
            }

         
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            
            
        }



        [Fact]
        public async void TestDeleteRequestAsync()
        {
            var Response = await _client.DeleteAsync("/api/notes/DeleteNote/1");
            Response.EnsureSuccessStatusCode();
        }



        //var note = new Notes
        //{
        //    Title = "title2",
        //    Label = new List<Label>
        //        {
        //            new Label{ LabelName="label21" },
        //            new Label{ LabelName="label22" }
        //        },
        //    IsPlainText = false
        //};




        //[Fact]
        //public async void Testput()
        //{
        //    var json = JsonConvert.SerializeObject(note);
        //    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
        //    var Response = await _client.PutAsync("api/Notes/EditNote/100", stringContent);
        //    var responsedata = Response.StatusCode;
        //    Assert.Equal(HttpStatusCode.OK, responsedata);
        //}



        //[Fact]
        //public async void TestDeleteNotFound()
        //{
        //    var response = await _client.DeleteAsync("api/Notes/DeleteNote/10");
        //    var responsecode = response.StatusCode;
        //    Assert.Equal(HttpStatusCode.NotFound, responsecode);
        //}


        //[Fact]
        //public async void TestDelete()
        //{
        //    var response = await _client.DeleteAsync("api/Notes/DeleteNote/1");
        //    var responsecode = response.StatusCode;
        //    Assert.Equal(HttpStatusCode.NoContent, responsecode);
        //}

        ////[Fact]
        ////public async void TestGetRequest()
        ////{
        ////    var response = await _client.GetAsync("/api/notes/GetNotes");
        ////    var responseBody = await response.Content.ReadAsStringAsync();
        ////    Console.WriteLine(responseBody.ToString());
        ////    Assert.Empty(responseBody.ToList());
        ////}







        //[Fact]
        //public async void TestGetResponseCodeRequest()
        //{
        //    var response = await _client.GetAsync("/api/notes/GetNotes");
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}




    }
}
