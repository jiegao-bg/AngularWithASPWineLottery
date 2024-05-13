using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;

namespace AngularWithASPWineLottery.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WineLotteryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<WineLotteryController> _logger;

        private int _ticketNumber = 100;

        private static readonly string FilePath = Path.Combine(Path.GetTempPath(), "Tickets.txt"); 

        public WineLotteryController(ILogger<WineLotteryController> logger)
        {
            _logger = logger;
            if (!System.IO.File.Exists(FilePath))
            {
                using (FileStream fs = System.IO.File.Create(FilePath))
                {
                }
            }
            
        }

        [HttpGet("{ticket}")]
        public async Task<ActionResult<int>> GetNumber()
        {
            var oldTicket = 0;
            var users = await ReadUsersFromFile(FilePath);

            foreach (var u in users)
            {
                oldTicket += u.Ticket;
            }

            var user = _ticketNumber -= oldTicket;
           
            return Ok(user);
        }

        [HttpGet(Name = "GetUser")]
        public async Task<ActionResult<IEnumerable<LotteryUser>>> Get()
        {
            var users = await ReadUsersFromFile(FilePath);
            
            return  Ok(users);
        }

        [HttpPost]
        public IActionResult PostMyData([FromBody] LotteryUser user)
        {
            // Handle the model, e.g., save to file
            user.Date = DateTime.Now;

            using var sw = new StreamWriter(FilePath, append: true);
            sw.WriteLine(user.Date);
            sw.WriteLine(user.Summary);
            sw.WriteLine(user.Name);
            sw.WriteLine(user.Id);
            sw.WriteLine(user.Ticket);
            sw.Close();

            return Ok(new { success = true, received = user });
        }

        //[HttpPost]
        //public async Task<ActionResult<bool>> PostData(LotteryUser user)
        //{
        //    // Handle the model, e.g., save to file
        //    user.Date = DateTime.Now;

        //    await using var sw = new StreamWriter(FilePath, append: true);
        //    await sw.WriteLineAsync(user.Date.ToShortDateString());
        //    await sw.WriteLineAsync(user.Summary);
        //    await sw.WriteLineAsync(user.Name);
        //    await sw.WriteLineAsync(user.Id);
        //    await sw.WriteLineAsync(user.Ticket.ToString());
        //    sw.Close();

        //    return true;
        //}

        // TODO: FOR DATABASE
        //    [HttpPost]                                                    
        //    public async Task<ActionResult<bool>> PostData(LotteryUser user) 
        //    {
        //        _context.LotteryPod.Add(user);
        //        await _context.SaveChangesAsync();

        //        return true;
        //    }

        private static async Task<List<LotteryUser>> ReadUsersFromFile(string filePath)
        {
            List<LotteryUser> users = new List<LotteryUser>();
            string[] lines = await System.IO.File.ReadAllLinesAsync(filePath);

            for (int i = 0; i < lines.Length; i += 5)
            {
                // Make sure there are enough lines left for a User object
                if (i + 4 < lines.Length)
                {
                    try
                    {
                        LotteryUser user = new LotteryUser
                        {
                            Date = DateTime.ParseExact(lines[i].Trim(), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                            Summary = lines[i + 1].Trim(),
                            Name = lines[i + 2].Trim(),
                            Id = lines[i + 3].Trim(),
                            Ticket = int.Parse(lines[i + 4].Trim())
                        };
                        users.Add(user);
                    }
                    catch (FormatException ex)
                    {
                        // Handle formatting exceptions (e.g., incorrect date format or ticket number)
                        Console.Error.WriteLine($"Error parsing user data: {ex.Message}");
                    }
                }
            }
            return users;
        }

    }
}
