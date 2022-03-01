using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class TagService : ITagService
    {
        private ITagRepository _tagRepo;
        public TagService(ITagRepository tagRepository)
        {
            _tagRepo = tagRepository;
        }
        public async Task<IEnumerable<TagDomainModel>> GetAll()
        {
            var result = await _tagRepo.GetAll();
            List<TagDomainModel> tags = new List<TagDomainModel>();
            foreach(var item in result)
            {
                tags.Add(new TagDomainModel
                {
                    Id = item.Id,
                    TagName = item.TagName
                });
            }

            return tags;
        }

        public async Task<ResponseModel<TagDomainModel>> GetById(object id)
        {
            var result = await _tagRepo.GetByIdAsync(id);
            if(result == null)
            {
                return new ResponseModel<TagDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Tag doesn't exist!"
                };
            }
            TagDomainModel tagDomainModel = new TagDomainModel
            {
                Id = result.Id,
                TagName = result.TagName
            };
            return new ResponseModel<TagDomainModel>
            {
                DomainModel = tagDomainModel,
                IsSuccessful = true
            };
        }

        public async Task<ResponseModel<TagDomainModel>> Create(TagDomainModel newTag)
        {
            Tag tagToAdd = new Tag();
            tagToAdd.TagName = newTag.TagName;

            var sameTag = await _tagRepo.SearchByName(tagToAdd.TagName);
            if (sameTag != null)
            {
                return new ResponseModel<TagDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Tag already exists!"
                };
            }
            var data = _tagRepo.Insert(tagToAdd);
            if(data == null)
            {
                return new ResponseModel<TagDomainModel>
                {
                    ErrorMessage = "Failed to create tag",
                    IsSuccessful = false
                };
            }

            _tagRepo.Save();

            TagDomainModel result = new TagDomainModel
            {
                Id = data.Id,
                TagName = data.TagName
            };
            return new ResponseModel<TagDomainModel>
            {
                IsSuccessful = true,
                DomainModel = result
            };
        }
    }
}
