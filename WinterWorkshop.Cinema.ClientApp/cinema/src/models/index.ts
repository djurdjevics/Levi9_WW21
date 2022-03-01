export interface IProjection {
  id: string;
  projectionTime: string;
  movieId: string;
  auditoriumName: string;
  bannerUrl?: string;
  movieTitle?: string;
  movieRating?: number;
  movieYear?: string;
  movie?: IMovie;
}

export interface IMovie {
  id: string;
  title: string;
  rating: number;
  year: string;
  bannerUrl?: string;
  current?: boolean;
  projections?: IProjection[];
  trailerUrl?: string;
}


export interface IMovieIMBD {
  id: string;
  title: string;
  rating: number;
  year: string;
  bannerUrl?: string;
  current?: boolean;
  projections?: IProjection[];
  poster?:string;
  plot?:string;
  trailer?:ITrailer;
}

export interface IImage{
  url: string;
}

export interface ITrailer{
  link: string;
}

export interface IProjectionFilter{
  cinemaId: number,
  auditoriumId: number,
  movieId?: string,
  dateTime?: string
}

export interface ICinema {
  id: string;
  name: string;
}

export interface IAuditorium {
  id: string;
  name: string;
  cinemaId?: string;
}

export interface ISeats {
  id: string;
  number: number;
  row: number;
}

export interface ICurrentReservationSeats {
  id: string;
}

export interface IReservedSeats {
  id: string;
}

export interface IUser {
  id: string;
  firstName: string;
  lastName: string;
  bonusPoints: string;
}

export interface IReservation {
  projectionId: string;
}

export interface ITag {
  tagId: string,
  tagName: string,
  movieId: string,
  movieTitle: string
}
