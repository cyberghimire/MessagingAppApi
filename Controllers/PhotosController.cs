using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MessagingApp.API.Data;
using MessagingApp.API.Dtos;
using MessagingApp.API.Helpers;
using MessagingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MessagingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController: ControllerBase
    {
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IMapper _mapper;
        private readonly IMessagingRepository _repo;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IMessagingRepository repository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._mapper = mapper;
            this._repo = repository;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("/{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);     //The Photo includes all the user's details as well, because that's the navigation property on our photo. 
                                                        //So, what we really want is the photo to return. 

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);  //We map the photoFromRepo to type PhotoForReturnDto because we don't want to return everything 
                                                                        //that's inside our photoFromRepo.
            return Ok(photo);

        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0){
                using (var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Url.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if(!userFromRepo.Photos.Any(u => u.IsMain))     //If there are no photos in the user's profile, then the uploaded photo becomes the main photo. 
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo); 
                return CreatedAtRoute("GetPhoto", new { id = photo.Id}, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{photoId}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var user = await _repo.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == photoId))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(photoId);

            if(photoFromRepo.IsMain)
                return BadRequest("This is already the main photo.");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll()){
                return NoContent();
            }

            return BadRequest("Could not set the photo as main photo.");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int photoId)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var user = await _repo.GetUser(userId);

            if(!user.Photos.Any(p => p.Id == photoId))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(photoId);

            if(photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo.");

            
            // await _repo.DeletePhoto(photoId);
            // if(await _repo.SaveAll()){
            //     return NoContent();
            // }
            
            return BadRequest("Could not delete the photo.");
            

        }


    }
}