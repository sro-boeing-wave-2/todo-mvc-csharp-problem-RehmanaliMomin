using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using WebApplication1;
using WebApplication1.Models;
using Xunit;

namespace XUnitTestProject1
{
    public class IntegrationTest
    {
        private HttpClient _client;
        List<Notes> notes;
        //Notes note;

        public IntegrationTest()
        {
            var host = new TestServer(
                new WebHostBuilder()
                .UseEnvironment("Testing")
                .UseStartup<Startup>());

            _client = host.CreateClient();

            notes = new List<Notes>();
            CreateList(notes);

        }

        public void CreateList(List<Notes> notes)
        {

            var note = new List<Notes>
            {
                new Notes
                {
                    Title = "title1",
                    Label = new List<Label>
                    {
                        new Label{LabelName="label1"},
                        new Label{LabelName="label2"}
                    },
                    IsPlainText = true
                },
                new Notes
                {
                    Title = "title2",
                    Label = new List<Label>
                    {
                        new Label{LabelName="label21"},
                        new Label{LabelName="label22"}
                    },
                    IsPlainText = false
                }
            };

            notes.AddRange(note.ToList());
            
        }



        [Fact]
        public async void TestGetRequest()
        {
            var response = await _client.GetAsync("/api/notes/GetNotes");
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal(2, responseBody.ToList().Count);
        }

        [Fact]
        public async void TestGetResponseCodeRequest()
        {
            var response = await _client.GetAsync("/api/notes/GetNotes");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        //[Fact]
        //public async void TestDeleteRequest()
        //{
        //    var response = await _client.GetAsync("/api/notes/DeleteNote/1");
        //    // var responseBody = await response.Content.ReadAsStringAsync();
        //    var response1 = await _client.GetAsync("/api/notes/GetNotes");
        //    var responseBody = await response1.Content.ReadAsStringAsync();
        //    Console.WriteLine(response.ToString());
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


        //}


        //[Fact]
        //public async void TestGetByTitleRequest()
        //{
        //    var response = await _client.GetAsync("/api/notes/GetAllByTitleName/title2");
        //    var fetchedNotes = await response.Content.ReadAsAsync<List<Notes>>();
        //    //var responseBody = await response.Content.ReadAsStringAsync();

        //    var note = new Notes
        //         {
        //            Title = "title2",
        //            Label = new List<Label>
        //            {
        //                new Label{ LabelName="label21" },
        //                new Label{ LabelName="label22" }
        //            },
        //            IsPlainText = false
        //        };

        //    // Convert.ChangeType(responseBody, note);
        //    Console.WriteLine(fetchedNotes.Count);
        //    Console.WriteLine(note.Title);
        //    Assert.Equal(note.Title, fetchedNotes[0].Title);
        //}



    }
}
