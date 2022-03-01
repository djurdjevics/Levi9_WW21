import React, { useEffect, useState } from "react";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../appSettings";
import { withRouter } from "react-router-dom";
import { Container, Row, Col, Card, Button } from "react-bootstrap";
import "./../../index.css";
import { IAuditorium, IProjection, ICinema, IMovie, IProjectionFilter } from "../../models";
import AllCurrentProjections from "./AllCurrentProjections";
import { State } from "react-notification-system";
import { getAllCinemas, getAllAuditoriums, getCurrentMoviesAndProjections, getAuditoriumsBySelectedCinema, getMoviesBySelectedAuditorium, getCurrentMoviesFilteredReturnProjections } from "../APICommunication";

interface IState {
  projections: IProjection[];
  movies: IMovie[];
  cinemas: ICinema[];
  auditoriums: IAuditorium[];
  filteredAuditoriums: IAuditorium[];
  filteredMovies: IMovie[];
  filteredProjections: IProjection[];
  dateTime: string;
  id: string;
  current: boolean;
  tag: string;
  titleError: string;
  yearError: string;
  submitted: boolean;
  isLoading: boolean;
  selectedCinema: boolean;
  selectedAuditorium: boolean;
  selectedMovie: boolean;
  selectedDate: boolean;
  date: Date;
  cinemaId: string;
  auditoriumId: string;
  movieId: string;
  name: string;
  filterProj: IProjectionFilter;
}

const Projection: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    projections: [
      {
        id: "",
        movieId: "",
        projectionTime: "",
        auditoriumName: "",
        movie: {
          id: "",
          bannerUrl: "",
          title: "",
          rating: 0,
          year: ""
        }
      },
    ],
    movies: [
      {
        id: "",
        bannerUrl: "",
        title: "",
        rating: 0,
        year: "",
        projections: [
          {
            id: "",
            movieId: "",
            projectionTime: "",
            auditoriumName: "",
          },
        ],
      },
    ],
    cinemas: [{ id: "", name: "" }],
    auditoriums: [
      {
        id: "",
        name: "",
      },
    ],
    filteredAuditoriums: [
      {
        id: "",
        name: "",
      },
    ],
    filteredMovies: [
      {
        id: "",
        bannerUrl: "",
        title: "",
        rating: 0,
        year: "",
      },
    ],
    filteredProjections: [
      {
        id: "",
        movieId: "",
        projectionTime: "",
        bannerUrl: "",
        auditoriumName: "",
        movieTitle: "",
        movieRating: 0,
        movieYear: "",
        movie: {
          id: "",
          bannerUrl: "",
          title: "",
          rating: 0,
          year: "",
          trailerUrl:""
        }
      },
    ],
    cinemaId: "",
    auditoriumId: "",
    movieId: "",
    dateTime: "",
    id: "",
    name: "",
    current: false,
    tag: "",
    titleError: "",
    yearError: "",
    submitted: false,
    isLoading: true,
    selectedCinema: false,
    selectedAuditorium: false,
    selectedMovie: false,
    selectedDate: false,
    date: new Date(),
    filterProj: {
      cinemaId: 0,
      auditoriumId: 0,
      movieId: "00000000-0000-0000-0000-000000000000"
    }
  });

  useEffect(() => {
    getCurrentMoviesAndProjections().then((data) => {
      if (data) {
        setState((prevData) => ({ ...prevData, movies: data }));
      }
    })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, isLoading: false });
      });
  }, []);
  useEffect(() => {
    getAllCinemas().then((data) => {
      if (data) {
        setState((prevData) => ({ ...prevData, cinemas: data }));
      }
    })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, isLoading: false });
      });
  }, []);
  useEffect(() => {
    getAllAuditoriums().then((data) => {
      if (data) {
        setState((prevData) => ({ ...prevData, auditoriums: data }));
      }
    })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, isLoading: false });
      });
  }, []);
  useEffect(() => {
    getCurrentFilteredMoviesAndProjections();
  }, []);

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setState({ ...state, submitted: true });
    getCurrentFilteredMoviesAndProjections();
  };

  const getCurrentFilteredMoviesAndProjections = () => {
    const { cinemaId, auditoriumId, movieId, dateTime } = state;
    if (cinemaId)
      state.filterProj.cinemaId = Number(cinemaId);
    if (auditoriumId)
      state.filterProj.auditoriumId = Number(auditoriumId);
    if (movieId)
      state.filterProj.movieId = movieId;
    if (dateTime)
      state.filterProj.dateTime = dateTime;
    console.log(state.filterProj);
    getCurrentMoviesFilteredReturnProjections(state.filterProj).then((data) => {
      if (data) {
        console.log(data);
        let movies = state.movies;
        let filteredMovies = data;
        for (let i = 0; i < movies.length; i++) {
          for (let j = 0; j < filteredMovies.length; j++) {
            if (movies[i].id === data[j].movieId) {
              data[j].bannerUrl = movies[i].bannerUrl;
            }
          }
        }
        setState((prevData) => ({ ...prevData, filteredProjections: data }))
        if(data.length === 0)
        {
            NotificationManager.warning("There is no projection for the entered filtering parameters!");
        }
      }
    }).catch((response) => {
      setState({ ...state, isLoading: false });
      console.log(response);
      NotificationManager.error(response.errorMessage);
    });
  }

  const fillFilterWithMovies = () => {
    if (state.selectedAuditorium) {
      return state.filteredMovies.map((movie) => {
        return (
          <option value={movie.id} key={movie.id}>
            {movie.title}
          </option>
        );
      });
    } else {
      return state.movies.map((movie) => {
        return (
          <option value={movie.id} key={movie.id}>
            {movie.title}
          </option>
        );
      });
    }
  };

  const fillFilterWithCinemas = () => {
    return state.cinemas.map((cinema) => {
      return (
        <option value={cinema.id} key={cinema.id}>
          {cinema.name}
        </option>
      );
    });
  };

  const fillFilterWithAuditoriums = () => {
    if (state.selectedCinema) {
      return state.filteredAuditoriums.map((auditorium) => {
        return <option value={auditorium.id}>{auditorium.name}</option>;
      });
    } else {
      return state.auditoriums.map((auditorium) => {
        return (
          <option value={auditorium.id} key={auditorium.id}>
            {auditorium.name}
          </option>
        );
      });
    }
  };

  const getAuditoriumsBySelectedCinemaId = (selectedCinemaId: string) => {
    if (selectedCinemaId === "none") {
      selectedCinemaId = "0";
      setState((prevData) => ({ ...prevData, selectedCinema: false, selectedAuditorium: false, auditoriumId: "0", cinemaId: "0", filteredAuditoriums: [], filteredMovies: [] }));
      fillFilterWithAuditoriums();
    }
    else {
      setState((prevData) => ({ ...prevData, cinemaId: selectedCinemaId, selectedAuditorim: false, auditoriumId: "0", filteredAuditoriums: [], filteredMovies: [] }));
      getAuditoriumsBySelectedCinema(selectedCinemaId).then((data) => {
        if (data) { setState((prevData) => ({ ...prevData, filteredAuditoriums: data, selectedCinema: true })); }
      })
        .catch((response) => {
          NotificationManager.error(response.message || response.statusText);
          setState({ ...state, isLoading: false });
        });
    }
  };

  const getMoviesBySelectedAuditoriumId = (selectedAuditoriumId: string) => {
    if (selectedAuditoriumId === "none") {
      selectedAuditoriumId = "0";
      setState((prevData) => ({ ...prevData, auditoriumId: "0", selectedAuditorium: false }));
    }
    else {
      setState((prevData) => ({ ...prevData, auditoriumId: selectedAuditoriumId }));
      getMoviesBySelectedAuditorium(selectedAuditoriumId).then((data) => {
        if (data) { setState((prevData) => ({ ...prevData, filteredMovies: data })); }
      }).catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, isLoading: false });
      });
    }
  };

  const setCheckedMovie = (selectedMovieId: string) => {
    if (selectedMovieId === "none") {
      selectedMovieId = "";
      setState((prevData) => ({ ...prevData, movieId: "00000000-0000-0000-0000-000000000000" }));
    }
    else {
      setState((prevData) => ({ ...prevData, selectedMovie: true, movieId: selectedMovieId }));
    }
  }

  const setDateTime = (selectedDateTime: string) => {
    console.log(selectedDateTime);
    setState({ ...state, selectedDate: true, dateTime: selectedDateTime })
  }

  const checkIfFiltered = () => {
    if (state.submitted) {
      return <AllCurrentProjections projections={state.filteredProjections} />
    } else {
      return <AllCurrentProjections projections={state.filteredProjections} />
    }
  };

  return (
    <Container>
      <h1 className="projections-title">Current projections</h1>
      <form
        id="name"
        name={state.name}
        onSubmit={handleSubmit}
        className="align-center"
      >
        <span className="filter-heading">Filter by:</span>
        <select
          onChange={(e) => getAuditoriumsBySelectedCinemaId(e.target.value)}
          name="cinemaId"
          id="cinema"
          className="select-dropdown"
        >
          <option value="none">Cinema</option>
          {fillFilterWithCinemas()}
        </select>
        <select
          onChange={(e) => getMoviesBySelectedAuditoriumId(e.target.value)}
          name="auditoriumId"
          id="auditorium"
          className="select-dropdown"
        // disabled = {state.selectedCinema === false}
        >
          {/* <option disabled={state.selectedAuditorium != false} value="none">Auditorium</option> */}
          <option value="none">Auditorium</option>
          {fillFilterWithAuditoriums()}
        </select>
        <select
          onChange={(e) => setCheckedMovie(e.target.value)}
          name="movieId"
          id="movie"
          className="select-dropdown"
        >
          <option value="none">Movie</option>
          {fillFilterWithMovies()}
        </select>
        <input
          onChange={(e) => setDateTime(e.target.value)
          }
          name="dateTime"
          type="date"
          id="date"
          className="input-date select-dropdown"
        />
        <button
          id="filter-button"
          className="btn-search"
          type="submit"
          onClick={() => setState({ ...state, submitted: true })}
        >
          Filter
        </button>
      </form>
      <Row className="justify-content-center">
        <Col>
          <Card className="card-width">{checkIfFiltered()}</Card>
        </Col>
      </Row>
    </Container>
  );
};

export default withRouter(Projection);
