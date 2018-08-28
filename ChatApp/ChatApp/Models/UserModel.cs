using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class UserModel : IdentityUser<int>
    {
        public UserModel(string userName) : base(userName)
        {
        }
        public UserModel()
        {
        }

        public ICollection<MessageModel> Messages { get; set; }
        public ICollection<ChannelModel> Channels { get; set; }
        public UserChannelModel UserChannel { get; set; }
    }
}
