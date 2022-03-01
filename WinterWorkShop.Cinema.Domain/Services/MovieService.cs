using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository _moviesRepository;
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly ITagService _tagService;
        private readonly IMovieWithTagRepository _movieWithTagRepository;
        private readonly ITagRepository _tagRepo;

        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionsRepository, ITagService tagService, IMovieWithTagRepository movieWithTagRepository, ITagRepository tagRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionsRepository = projectionsRepository;
            _tagService = tagService;
            _movieWithTagRepository = movieWithTagRepository;
            _tagRepo = tagRepository;
        }

        public IEnumerable<MovieDomainModel> GetCurrentMovies(bool? isCurrent)
        {
            var data = _moviesRepository.GetCurrentMovies();


            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year,
                    HasOscar = item.HasOscar,
                    TrailerUrl = item.TrailerUrl,
                    BannerUrl = item.BannerUrl

                };
                result.Add(model);
            }

            return result;

        }



        public async Task<ResponseModel<MovieDomainModel>> GetMovieByIdAsync(Guid id)
        {
            var data = await _moviesRepository.GetByIdAsync(id);

            if (data == null)
            {
                return new ResponseModel<MovieDomainModel>
                {
                    ErrorMessage = "Movie doesn't exists!",
                    IsSuccessful = false
                };
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Current = data.Current,
                Rating = data.Rating ?? 0,
                Title = data.Title,
                Year = data.Year,
                HasOscar = data.HasOscar,
                TrailerUrl = data.TrailerUrl,
                BannerUrl = data.BannerUrl
            };

            return new ResponseModel<MovieDomainModel>
            {
                DomainModel = domainModel,
                IsSuccessful = true
            };
        }

        public async Task<ResponseModel<MovieDomainModel>> AddMovie(MovieDomainModel newMovie)
        {
            Movie movieToCreate = new Movie()
            {
                Title = newMovie.Title,
                Current = newMovie.Current,
                Year = newMovie.Year,
                Rating = newMovie.Rating,
                HasOscar = newMovie.HasOscar,
                TrailerUrl = newMovie.TrailerUrl,
                BannerUrl = newMovie.BannerUrl

            };


            var data = _moviesRepository.Insert(movieToCreate);
            if (data == null)
            {
                return new ResponseModel<MovieDomainModel>
                {
                    ErrorMessage = "Failed to add movie!",
                    IsSuccessful = false
                };
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0,
                HasOscar = data.HasOscar,
                TrailerUrl = data.TrailerUrl,
                Tags = new List<TagDomainModel>(),

                BannerUrl = data.BannerUrl

            };



            foreach (var item in newMovie.Tags)
            {
                var sameMovie = await _tagRepo.SearchByName(item.TagName);

                if (sameMovie == null)
                {
                    var insertedTag = _tagRepo.Insert(new Tag
                    {
                        Id = new Guid(),
                        TagName = item.TagName
                    });
                    _tagRepo.Save();

                    _movieWithTagRepository.Insert(new MovieWithTag
                    {
                        MovieId = data.Id,
                        TagId = insertedTag.Id
                    });

                    domainModel.Tags.Add(new TagDomainModel
                    {
                        Id = insertedTag.Id,
                        TagName = insertedTag.TagName,
                    });
                }
                else
                {
                    _movieWithTagRepository.Insert(new MovieWithTag
                    {
                        MovieId = data.Id,
                        TagId = sameMovie.Id
                    });
                    domainModel.Tags.Add(new TagDomainModel
                    {
                        Id = sameMovie.Id,
                        TagName = sameMovie.TagName,
                    });
                }
            }
            _movieWithTagRepository.Save();


            return new ResponseModel<MovieDomainModel>
            {
                DomainModel = domainModel,
                IsSuccessful = true
            };
        }

        public async Task<ResponseModel<MovieDomainModel>> UpdateMovie(MovieDomainModel updateMovie)
        {

            var movieToUpdate = await _moviesRepository.GetByIdAsync(updateMovie.Id);
            if (movieToUpdate == null)
            {
                return new ResponseModel<MovieDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Failed to find movie to update!"
                };
            }
            Movie movie = new Movie()
            {
                Id = updateMovie.Id,
                Title = updateMovie.Title,
                Current = updateMovie.Current,
                Year = updateMovie.Year,
                Rating = updateMovie.Rating,
                TrailerUrl = movieToUpdate.TrailerUrl,
                HasOscar = updateMovie.HasOscar,
                BannerUrl = movieToUpdate.BannerUrl
            };



            var data = _moviesRepository.Update(movie);

            if (data == null)
            {
                return new ResponseModel<MovieDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Failed to update movie!"
                };
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0,
                HasOscar = data.HasOscar,
                TrailerUrl = data.TrailerUrl,
                BannerUrl = data.BannerUrl
            };

            return new ResponseModel<MovieDomainModel>
            {
                IsSuccessful = true,
                DomainModel = domainModel
            };
        }

        public async Task<ResponseModel<MovieDomainModel>> DeleteMovie(Guid id)
        {

            var checkProjections = _projectionsRepository.GetByMovieId(id);

            foreach (var item in checkProjections)
            {
                if (item.ProjectionTime > DateTime.Now)
                {
                    return new ResponseModel<MovieDomainModel>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Movie has projections in future!"
                    };
                }
            }


            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0,
                TrailerUrl = data.TrailerUrl,
                HasOscar = data.HasOscar,
                BannerUrl = data.BannerUrl

            };

            return new ResponseModel<MovieDomainModel>
            {
                DomainModel = domainModel,
                IsSuccessful = true
            };
        }

        public async Task<ResponseModel<MovieDomainModel>> ActivateDeactivateMovie(Guid id)
        {
            var movie = await _moviesRepository.GetByIdAsync(id);
            var checkProjections = _projectionsRepository.GetByMovieId(id);

            foreach (var item in checkProjections)
            {
                if (item.ProjectionTime > DateTime.Now)
                {
                    return new ResponseModel<MovieDomainModel>
                    {
                        IsSuccessful = false,
                        ErrorMessage = "Movie has projections in future!"
                    };
                }
            }

            if (movie == null)
            {
                return new ResponseModel<MovieDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Movie doesn't exist!"
                };
            }
            movie.Current = !movie.Current;

            MovieDomainModel result = new MovieDomainModel
            {
                Id = movie.Id,
                Current = movie.Current,
                Rating = (double)movie.Rating,
                Title = movie.Title,
                Year = movie.Year,
                HasOscar = movie.HasOscar,
                TrailerUrl = movie.TrailerUrl,
                BannerUrl = movie.BannerUrl
            };

            _moviesRepository.Update(movie);
            _moviesRepository.Save();
            return new ResponseModel<MovieDomainModel>
            {
                IsSuccessful = true,
                DomainModel = result
            };
        }

        public async Task<IEnumerable<MovieDomainModel>> GetAllMovies()
        {
            var data = await _moviesRepository.GetAll();

            var result = new List<MovieDomainModel>();
            foreach (var item in data)
            {
                result.Add(
                    new MovieDomainModel
                    {
                        Id = item.Id,
                        Current = item.Current,
                        Rating = (double)item.Rating,
                        Title = item.Title,
                        TrailerUrl = item.TrailerUrl,
                        Year = item.Year,
                        HasOscar = item.HasOscar,
                        BannerUrl = item.BannerUrl
                    });
            }
            return result;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetTopTenMovies()
        {
            var data = await _moviesRepository.GetTopTenMovies();
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            foreach (var item in data)
            {
                result.Add(new MovieDomainModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Current = item.Current,
                    Rating = (double)item.Rating,
                    Year = item.Year,
                    HasOscar = item.HasOscar,
                    TrailerUrl = item.TrailerUrl,
                    BannerUrl = item.BannerUrl
                });
            }

            return result;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetTopTenMoviesByYear(int year)
        {
            var data = await _moviesRepository.GetTopTenMoviesByYear(year);
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            foreach (var item in data)
            {
                result.Add(new MovieDomainModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Current = item.Current,
                    HasOscar = item.HasOscar,
                    Rating = item.Rating ?? 0,
                    TrailerUrl = item.TrailerUrl,
                    Year = item.Year,
                    BannerUrl = item.BannerUrl
                });
            }
            return result;
        }



        public async Task<IEnumerable<MovieDomainModel>> GetByTag(object tagName)
        {
            var data = await _movieWithTagRepository.GetByTagName(tagName);
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            foreach (var item in data)
            {
                result.Add(new MovieDomainModel
                {
                    Id = item.Movie.Id,
                    Current = item.Movie.Current,
                    HasOscar = item.Movie.HasOscar,
                    Rating = item.Movie.Rating ?? 0,
                    TrailerUrl = item.Movie.TrailerUrl,
                    Title = item.Movie.Title,
                    Year = item.Movie.Year,
                    BannerUrl = item.Movie.BannerUrl,
                    Tags = new List<TagDomainModel> {
                        new TagDomainModel
                        {
                            Id = item.TagId,
                            TagName = item.Tag.TagName
                        }
                    }
                });
            }
            return result;

        }

        public async Task<IEnumerable<MovieDomainModel>> GetByTitle(string title)
        {
            var data = await _moviesRepository.GetByTitle(title);
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            foreach (var item in data)
            {
                result.Add(new MovieDomainModel
                {
                    Id = item.Id,
                    Current = item.Current,
                    HasOscar = item.HasOscar,
                    Rating = item.Rating ?? 0,
                    TrailerUrl = item.TrailerUrl,
                    Title = item.Title,
                    Year = item.Year,
                    BannerUrl = item.BannerUrl,
                });
            }
            return result;
        }

        public async Task<IEnumerable<MovieDomainModel>> GetByYear(string year)
        {
            var data = await _moviesRepository.GetByYear(year);
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            foreach (var item in data)
            {
                var tags = await _movieWithTagRepository.GetByMovieID(item.Id);
                result.Add(new MovieDomainModel
                {
                    Id = item.Id,
                    Current = item.Current,
                    HasOscar = item.HasOscar,
                    Rating = item.Rating ?? 0,
                    TrailerUrl = item.TrailerUrl,
                    Title = item.Title,
                    Year = item.Year,
                    BannerUrl = item.BannerUrl,
                });
            }

            return result;
        }

        public async Task<ResponseModel<IEnumerable<MovieDomainModel>>> GetMoviesByAuditoriumId(int id)
        {
            var projections = _projectionsRepository.GetByAuditoriumId(id);

            if (projections == null)
                return new ResponseModel<IEnumerable<MovieDomainModel>>()
                {
                    DomainModel = null,
                    ErrorMessage = Messages.PROJECTION_GET_ALL_PROJECTIONS_ERROR,
                    IsSuccessful = false
                };
            var movies = projections.Select(moviee => new MovieDomainModel()
            {
                Current = moviee.Movie.Current,
                HasOscar = moviee.Movie.HasOscar,
                Id = moviee.Movie.Id,
                Rating = (double)moviee.Movie.Rating,
                TrailerUrl = moviee.Movie.TrailerUrl,
                Title = moviee.Movie.Title,
                Year = moviee.Movie.Year,
                BannerUrl = moviee.Movie.BannerUrl
            }).ToList();

            return new ResponseModel<IEnumerable<MovieDomainModel>>()
            {
                ErrorMessage = null,
                IsSuccessful = true,
                DomainModel = movies
            };

        }
    }
}
