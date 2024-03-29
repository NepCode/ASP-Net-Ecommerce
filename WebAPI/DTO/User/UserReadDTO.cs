﻿namespace WebAPI.DTO.User
{
    public class UserReadDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Admin { get; set; }
    }
}
