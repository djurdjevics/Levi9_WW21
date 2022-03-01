import React, { useEffect, useState } from "react";
import { withRouter } from "react-router-dom";
import {
  FormGroup,
  FormControl,
  Button,
  Container,
  Row,
  Col,
  FormText,
} from "react-bootstrap";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../../appSettings";
import { YearPicker } from "react-dropdown-date";
import{ getMovieById, updateMovieAPI } from "../../APICommunication";

interface IState {
  title: string;
  year: string;
  rating: number;
  id: string;
  current: boolean;
  titleError: string;
  yearError: string;
  submitted: boolean;
  canSubmit: boolean;
}

const EditMovie: React.FC = (props: any) => {
  const { id } = props.match.params;

  const [state, setState] = useState<IState>({
    title: "",
    year: "",
    rating: 0,
    id: "",
    current: false,
    titleError: "",
    yearError: "",
    submitted: false,
    canSubmit: true,
  });

  const getMovie = (movieId: string) => {

    getMovieById(movieId).then((data)=>{
      if(data){
        setState({...state,
          title:data.title,
          year:data.year,
          rating:Math.round(data.rating),
          current:data.current,
          id:data.id + "",});
      }
      else{
        NotificationManager.error(data.message||data.statusText);
        setState({...state,submitted:false});
      }
    })

  };

  useEffect(() => {
    getMovie(id);
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setState({ ...state, [id]: value });
    validate(id, value);
  };

  const validate = (id: string, value: string) => {
    if (id === "title") {
      if (value === "") {
        setState((prevData)=>({
          ...prevData,
          titleError: "Fill in movie title",
          canSubmit: false,
        }));
      } else {
        setState((prevData)=>({ ...prevData,title:value,titleError: "", canSubmit: true }));
      }
    }

    if (id === "year") {
      const yearNum = +value;
      if (!value || value === "" || yearNum < 1895 || yearNum > 2100) {
        setState((prevData)=>({
          ...prevData,
          yearError: "Please chose valid year",
          canSubmit: false,
        }));
      } else {
        setState((prevData)=>({ ...prevData,year:value,yearError: "", canSubmit: true }));
      }
    }
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setState({ ...state, submitted: true });
    if (state.title && state.year && state.rating) {
      updateMovie();
    } else {
      NotificationManager.error("Please fill in data");
      setState({ ...state, submitted: false });
    }
  };

  const handleYearChange = (year: string) => {
    setState({ ...state, year: year });
    validate("year", year);
  };

  const updateMovie = () => {
    const data = {
      Title: state.title,
      Year: +state.year,
      Current: state.current === true,
      Rating: +state.rating,
    };

    updateMovieAPI(state.id,data).then((response)=>{
      if(response){
        props.history.goBack();
        NotificationManager.success("Successfully edited movie!");
      }
      else{
        NotificationManager.error(response.message || response.statusText);
      }
    })
    
  };

  return (
    <Container>
      <Row>
        <Col>
          <h1 className="form-header">Edit Existing Movie</h1>
          <form onSubmit={handleSubmit}>
            <FormGroup>
              <FormControl
                id="title"
                type="text"
                placeholder="Movie Title"
                value={state.title}
                onChange={handleChange}
              />
              <FormText className="text-danger">{state.titleError}</FormText>
            </FormGroup>
            <FormGroup>
              <YearPicker
                defaultValue={"Select Movie Year"}
                start={1895}
                end={2120}
                reverse
                required={true}
                disabled={false}
                value={state.year}
                onChange={(year: string) => {
                  handleYearChange(year);
                }}
                id={"year"}
                name={"year"}
                classes={"form-control"}
                optionClasses={"option classes"}
              />
              <FormText className="text-danger">{state.yearError}</FormText>
            </FormGroup>
            <FormGroup>
              <FormControl
                as="select"
                placeholder="Rating"
                id="rating"
                value={state.rating.toString()}
                onChange={handleChange}
              >
                <option value="1">1</option>
                <option value="2">2</option>
                <option value="3">3</option>
                <option value="4">4</option>
                <option value="5">5</option>
                <option value="6">6</option>
                <option value="7">7</option>
                <option value="8">8</option>
                <option value="9">9</option>
                <option value="10">10</option>
              </FormControl>
            </FormGroup>
            <Button
              type="submit"
              disabled={state.submitted || !state.canSubmit}
              block
            >
              Edit Movie
            </Button>
          </form>
        </Col>
      </Row>
    </Container>
  );
};

export default withRouter(EditMovie);
