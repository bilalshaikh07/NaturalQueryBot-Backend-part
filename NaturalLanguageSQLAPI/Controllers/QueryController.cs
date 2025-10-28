using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalLanguageSQLAPI.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NaturalLanguageSQLAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly ChatbotContext _db;
        private readonly OpenAIAPI _openAI;

        public QueryController(ChatbotContext db, OpenAIAPI openAI)
        {
            _db = db;
            _openAI = openAI;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question is required.");

            var sql = await TranslateQuestionToSQL(request.Question);

            var result = await _db.Customers.FromSqlRaw(sql).ToListAsync();

            return Ok(result);
        }

        private async Task<string> TranslateQuestionToSQL(string question)
        {
            var chat = _openAI.Chat.CreateConversation();
            chat.AppendSystemMessage(@"
You are a SQL expert. Convert the following natural language question into a valid and secure SQL SELECT query.

Use only this schema:

Customers(CustomerId, FirstName, LastName, Email, City, Country)
Products(ProductId, ProductName, Category, Price)
Orders(OrderId, CustomerId, ProductId, OrderDate, Quantity)

🎯 Requirements:
- Only generate SQL SELECT queries
- Use JOINs when needed
- Do NOT use semicolons or comments
- Return only the SQL query, no explanation
");

            chat.AppendUserInput(question);

            var response = await chat.GetResponseFromChatbotAsync();
            return response.Trim();
        }
    }

    public class QueryRequest
    {
        public string Question { get; set; }
    }
}
