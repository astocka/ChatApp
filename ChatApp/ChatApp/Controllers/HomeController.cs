using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Context;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        protected UserManager<UserModel> UserManager { get; }
        private readonly EFContext _context;

        public HomeController(UserManager<UserModel> userManager, EFContext context)
        {
            UserManager = userManager;
            _context = context;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("ChannelDetails", "Home", new { id = 1 });
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: Home/ChannelDetails/1
        public async Task<IActionResult> ChannelDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelModel = await _context.Channels.Include(c => c.Messages)
                .ThenInclude(u => u.User)
                .SingleOrDefaultAsync(m => m.ID == id);

            if (channelModel == null)
            {
                return NotFound();
            }

            ViewBag.UserNr = UserManager.GetUserId(User);
            ViewBag.UserName = UserManager.GetUserName(User);
            
            return View(channelModel);
        }

        // GET: Home/ManageChannels
        public async Task<IActionResult> ManageChannels()
        {
            return View(await _context.Channels.ToListAsync());
        }

        // GET: Home/CreateChannel
        public IActionResult CreateChannel()
        {
            return View();
        }

        // POST: Home/CreateChannel
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChannel([Bind("ID,Name,Color")] ChannelModel channelModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(channelModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(channelModel);
        }

        // GET: Home/EditChannel/1
        public async Task<IActionResult> EditChannel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelModel = await _context.Channels.SingleOrDefaultAsync(m => m.ID == id);
            if (channelModel == null)
            {
                return NotFound();
            }
            return View(channelModel);
        }

        // POST: Home/EditChannel/1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChannel(int id, [Bind("ID,Name,Color")] ChannelModel channelModel)
        {
            if (id != channelModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(channelModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChannelModelExists(channelModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(channelModel);
        }

        // GET: Home/DeleteChannel/5
        public async Task<IActionResult> DeleteChannel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channelModel = await _context.Channels
                .SingleOrDefaultAsync(m => m.ID == id);
            if (channelModel == null)
            {
                return NotFound();
            }

            return View(channelModel);
        }

        // POST: Home/DeleteChannel/1
        [HttpPost, ActionName("DeleteChannel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChannelConfirmed(int id)
        {
            var channelModel = await _context.Channels.SingleOrDefaultAsync(m => m.ID == id);
            _context.Channels.Remove(channelModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChannelModelExists(int id)
        {
            return _context.Channels.Any(e => e.ID == id);
        }

        // GET: Home/CreateMessage/1
        public IActionResult CreateMessage(int id)
        {
            MessageModel message = new MessageModel();
            message.Channel = _context.Channels.SingleOrDefault(c => c.ID == id);

            ViewBag.UserNr = UserManager.GetUserId(User);
            return View(message);
        }

        // POST: Home/CreateMessage/1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMessage([Bind("Message")] MessageModel messageModel, int id)
        {
            messageModel.Channel = _context.Channels.SingleOrDefault(c => c.ID == id);
            messageModel.Created = DateTime.Now;
            messageModel.User = await UserManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                _context.Add(messageModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("ChannelDetails", "Home", new { id = messageModel.Channel.ID });
            }
            return View(messageModel);
        }

        // GET: Home/EditMessage/1
        public async Task<IActionResult> EditMessage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.Include(m => m.Channel)
                .SingleOrDefaultAsync(c => c.ID == id);

            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Home/EditMessage/1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMessage(int id, [Bind("ID,Message,Created")] MessageModel messageModel)
        {
            messageModel.Channel = _context.Channels.SingleOrDefault(c => c.ID == id);
            messageModel.Created = DateTime.Now;
            messageModel.User = await UserManager.GetUserAsync(User);

            if (id != messageModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageModelExists(messageModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(messageModel);
        }

        // GET: Home/DeleteMessage/1
        public async Task<IActionResult> DeleteMessage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.Include(c => c.Channel)
              .SingleOrDefaultAsync(m => m.ID == id);

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Home/DeleteMsgConfirmed/1
        [HttpPost, ActionName("DeleteMessage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMsgConfirmed(int id)
        {
            var messageModel = await _context.Messages.SingleOrDefaultAsync(m => m.ID == id);
            _context.Messages.Remove(messageModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageModelExists(int id)
        {
            return _context.Messages.Any(e => e.ID == id);
        }
    }
}
