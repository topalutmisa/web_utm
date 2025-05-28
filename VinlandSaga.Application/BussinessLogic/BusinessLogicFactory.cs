using System;
using VinlandSaga.Application.BussinessLogic.Interfaces;
using VinlandSaga.Application.BussinessLogic.BLogic;

namespace VinlandSaga.Application.BussinessLogic
{
    public sealed class BusinessLogicFactory
    {
        private static readonly Lazy<BusinessLogicFactory> _instance = 
            new Lazy<BusinessLogicFactory>(() => new BusinessLogicFactory());
        
        public static BusinessLogicFactory Instance => _instance.Value;
        
        private BusinessLogicFactory() { }
        
        public IUserBL GetUserBL()
        {
            return new UserBL();
        }
        
        public IForumBL GetForumBL()
        {
            return new ForumBL();
        }
        
        public ICharacterBL GetCharacterBL()
        {
            return new CharacterBL();
        }
        
        public INewsBL GetNewsBL()
        {
            return new NewsBL();
        }
    }
} 