import React, { useEffect, useState } from "react";
import { withRouter } from "react-router-dom";
import {
  FormGroup,
  Button,
  Container,
  Row,
  Col,
  FormText,
} from "react-bootstrap";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../../appSettings";
import { Typeahead } from "react-bootstrap-typeahead";
import DateTimePicker from "react-datetime-picker";
import { IAuditorium, IMovie } from "../../../models";
import { addNewProjection, getAllAuditoriums, getCurrentMoviesAndProjections } from "../../APICommunication";
interface IState {
  projectionTime: string;
  movieId: string;
  auditoriumId: string;
  submitted: boolean;
  projectionTimeError: string;
  movieIdError: string;
  auditoriumIdError: string;
  movies: IMovie[];
  auditoriums: IAuditorium[];
  canSubmit: boolean;
  choosedDate: string;
}

const NewProjection: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    projectionTime: "",
    movieId: "",
    auditoriumId: "",
    submitted: false,
    projectionTimeError: "",
    movieIdError: "",
    auditoriumIdError: "",
    movies: [
      {
        id: "",
        bannerUrl: "",
        rating: 0,
        title: "",
        year: "",
      },
    ],
    auditoriums: [
      {
        id: "",
        name: "",
      },
    ],
    canSubmit: true,
    choosedDate: ""
  });

  useEffect(() => {
    getCurrentMoviesAndProjections().then((data)=>{
      if (data) {
        setState((prevData) => ({...prevData, movies:data}));
      }
    })
    .catch((response) => {
      NotificationManager.error(response.message || response.statusText);
      setState((prevData) => ({...prevData, submitted:false}));
    });
    
    getAllAuditoriums().then((data) =>{
      if (data) {
        setState((prevData) => ({...prevData, auditoriums:data}));
      }
    })
    .catch((response) => {
      NotificationManager.error(response.message || response.statusText);
      setState((prevData) => ({...prevData, submitted:false}));
    });
  }, []);

  const handleChange = (e) => {
    const { id, value } = e.target;
    setState({ ...state, [id]: value });
  };

  const validate = (id, value) => {
    if (id === "projectionTime") {
      if (!value) {
        setState((prevData) => ({...prevData, projectionTimeError: "Chose projection time", canSubmit: false}));
      } else {
        setState((prevData) => ({...prevData, projectionTimeError: "", canSubmit: true}));
      }
    } else if (id === "movieId") {
      if (!value) {
        setState((prevData) => ({...prevData,  movieIdError: "Please chose movie from dropdown", canSubmit: false}));
      } else {
        setState((prevData) => ({...prevData,  movieIdError: "", canSubmit: true }));
      }
    } else if (id === "auditoriumId") {
      if (!value) {
        setState((prevData) => ({...prevData,   auditoriumIdError: "Please chose auditorium from dropdown", canSubmit: false }));
      } else {
        setState((prevData) => ({...prevData,   auditoriumIdError: "", canSubmit: true }));
      }
    }
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setState((prevData) => ({...prevData, submitted:true}));

    if (state.movieId && state.auditoriumId && state.projectionTime) {
      addProjection();
    } else {
      NotificationManager.error("Please fill in data");
      setState((prevData) => ({...prevData, submitted:false}));
    }
  };

  const addProjection = () => {
    const data = {
      movieId: state.movieId,
      auditoriumId: state.auditoriumId,
      projectionTime: state.projectionTime.toLocaleString(),
    };

    addNewProjection(data).then((respon)=>{
      if(respon.id)
      {
        NotificationManager.success("New projection added!");
        props.history.push(`AllProjections`);
      }
      if(respon.errorMessage){
        NotificationManager.error(respon.errorMessage);
        setState((prevData) => ({...prevData, submitted:false}));
      }
      })
      .catch((response) => {
        NotificationManager.error(response.errorMessage);
        setState((prevData) => ({...prevData, submitted:false}));
      });
  };

  const onMovieChange = (movies: IMovie[]) => {
    if (movies[0]) {
      setState((prevData) => ({...prevData, movieId: movies[0].id}));
      validate("movieId", movies[0]);
    } else {
      validate("movieId", null);
    }
  };

  const onAuditoriumChange = (auditoriums: IAuditorium[]) => {
    if (auditoriums[0]) {
      setState((prevData) => ({...prevData, auditoriumId: auditoriums[0].id }));
      validate("auditoriumId", auditoriums[0]);
    } else {
      validate("auditoriumId", null);
    }
  };

  const onDateChange = (date: string) =>{
    setState((prevData) => ({...prevData, projectionTime: date}));
  }
      
  return (
    <Container>
      <Row>
        <Col>
          <h1 className="form-header projections-title">Add Projection</h1>
          <form onSubmit={handleSubmit}>
            <FormGroup>
              <Typeahead
                labelKey="title"
                options={state.movies}
                placeholder="Choose a movie..."
                id="movie"
                className="add-new-form"
                onChange={(e) => {
                  onMovieChange(e);
                }}
              />
              <FormText  className="text-danger add-new-form">{state.movieIdError}</FormText>
            </FormGroup>
            <FormGroup>
              <Typeahead
                labelKey="name"
                className="add-new-form"
                options={state.auditoriums}
                placeholder="Choose auditorium..."
                id="auditorium"
                onChange={(e) => {
                  onAuditoriumChange(e);
                }}
              />
              <FormText  className="text-danger add-new-form">
                {state.auditoriumIdError}
              </FormText>
            </FormGroup>
            <FormGroup>
              <DateTimePicker
                className="form-control add-new-form"
                format="yyyy/MM/d HH:mm"
                onChange={onDateChange} value={state.projectionTime}
              />
              <FormText  className="text-danger add-new-form">
                {state.projectionTimeError}
              </FormText>
            </FormGroup>
            <Button
              className="btn-add-new"
              type="submit"
              disabled={state.submitted || !state.canSubmit}
              block
            >
              Add
            </Button>
          </form>
        </Col>
      </Row>
    </Container>
  );
};

export default withRouter(NewProjection);
