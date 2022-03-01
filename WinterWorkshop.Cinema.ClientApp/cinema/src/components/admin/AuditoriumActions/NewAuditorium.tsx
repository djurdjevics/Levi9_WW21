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
import { Typeahead } from "react-bootstrap-typeahead";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCouch } from "@fortawesome/free-solid-svg-icons";
import "./../../../index.css";
import { ICinema } from "../../../models";
import { type } from "os";

interface IState {
  cinemaId: string;
  auditName: string;
  seatRows: number;
  numberOfSeats: number;
  cinemas: ICinema[];
  cinemaIdError: string;
  auditNameError: string;
  seatRowsError: string;
  numOfSeatsError: string;
  submitted: boolean;
  canSubmit: boolean;
}

const NewAuditorium: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    cinemaId: "",
    auditName: "",
    seatRows: 0,
    numberOfSeats: 0,
    cinemas: [
      {
        id: "",
        name: "",
      },
    ],
    cinemaIdError: "",
    auditNameError: "",
    seatRowsError: "",
    numOfSeatsError: "",
    submitted: false,
    canSubmit: true,
  });

  const getCinemas = () => {
    const requestOptions = {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
    };

    fetch(`${serviceConfig.baseURL}/api/Cinemas`, requestOptions)
      .then((response) => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then((data) => {
        if (data) {
          setState({ ...state, cinemas: data });
        }
      })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, submitted: false });
      });
  };

  useEffect(() => {
    getCinemas();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    validate(id, value);
    setState((prevData)=>({...prevData, [id]:value}));
  };
  const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setState({ ...state, auditName: e.target.value });
  };
  
  const handleNumRowsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    validate(id, value);
    setState((prevData)=>({...prevData, seatRows: Number(value)}));
  }; 

  const handleNumSeatsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    validate(id, value);
    setState((prevData)=>({...prevData, numberOfSeats: Number(value)}));
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setState((prevData) =>({ ...prevData, submitted: true }));
    if (
      state.auditName &&
      state.numberOfSeats &&
      state.cinemaId &&
      state.seatRows
    ) {
      addAuditorium();
    } else {
      NotificationManager.error("Please fill form with data.");
      setState((prevData) => ({ ...prevData, submitted: false }));
    }
  };

  const validate = (id: string, value: string | null) => {
    if (id === "auditName") {
      if (value === "") {
        setState((prevData) => ({
          ...prevData,
          auditNameError: "Fill in auditorium name",
          canSubmit: false,
        }));
      } else {
        setState((prevData) =>({ ...prevData, auditNameError: "", canSubmit: true }));
      }
    } 
     else if (id === "numberOfSeats" && value) {
      const seatsNum = +value;
      if (seatsNum > 20 || seatsNum < 3) {
        setState((prevData) =>({
          ...prevData,
          numOfSeatsError: "Seats number can be in between 1 and 20.",
          canSubmit: false,
        }));
      } else {
        setState((prevData) =>({ ...prevData, numOfSeatsError: "", canSubmit: true }));
      }
    } 
    else if (id === "seatRows" && value) {
      const seatsNum = +value;
      if (seatsNum > 20 || seatsNum < 3) {
        setState((prevData) =>({
          ...prevData,
          seatRowsError: "Seats number can be in between 1 and 20.",
          canSubmit: false,
        }));
      } else {
        setState((prevData) =>({ ...prevData, seatRowsError: "", canSubmit: true }));
      }
    } 
    else if (id === "cinemaId") {
      if (!value) {
        setState((prevData) =>({
            ...prevData,
          cinemaIdError: "Please choose cinema from dropdown list.",
          canSubmit: false,
        }));
      } else {
        setState((prevData) =>({ ...prevData, cinemaIdError: "", canSubmit: true }));
      }
    }
  };

  const addAuditorium = () => {
    const data = {
      cinemaId: state.cinemaId,
      auditName: state.auditName,
      seatRows: +state.seatRows,
      numberOfSeats: +state.numberOfSeats,      
    };
    
    const requestOptions = {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
      body: JSON.stringify(data),
    };

    console.log(requestOptions);

    fetch(`${serviceConfig.baseURL}/api/auditoriums`, requestOptions)
      .then((response) => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.statusText;
      })
      .then((result) => {
        NotificationManager.success("Successfuly added new auditorium!");
        props.history.push("AllAuditoriums");
      })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState((prevData) =>({
          ...prevData, submitted: false }));
      });
  };

  const onCinemaChange = (cinemas: ICinema[]) => {
    
    if (cinemas[0]) {
      setState((prevData) =>({ ...prevData, cinemaId: cinemas[0].id }));    
      validate("cinemaId", cinemas[0].id);
      
    } else {
      validate("cinemaId", null);
    }
  };

  const renderSeats = (seats, row) => {
    let renderedSeats: JSX.Element[] = [];
    for (let i = 0; i < seats; i++) {
      renderedSeats.push(
        <td id="test" className="rendering-seats" key={`row${row}-seat${i}`}>
          <FontAwesomeIcon className="fa-2x couch-icon" icon={faCouch} />
        </td>
      );
    }
    return renderedSeats;
  };

  const renderRows = (rows: number, seats: number) => {
    const rowsRendered: JSX.Element[] = [];
    for (let i = 0; i < rows; i++) {
      rowsRendered.push(<tr key={i}>{renderSeats(seats, i)}</tr>);
    }
    return rowsRendered;
  };

  return (
    <Container>
      <Row>
        <Col>
          <h1 className="form-header">Add Auditorium</h1>
          <form onSubmit={handleSubmit}>
            <FormGroup>
              <FormControl
                id="auditName"
                type="text"
                placeholder="Auditorium Name"
                value={state.auditName}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleNameChange(e)}
                }
                className="add-new-form"
              />
              <FormText className="text-danger">
                {state.auditNameError}
              </FormText>
            </FormGroup>
            <FormGroup>
              <Typeahead
                labelKey="name"
                className="add-new-form"
                options={state.cinemas}
                placeholder="Choose a cinema..."
                id="browser"
                onChange={(e: ICinema[]) => {
                  onCinemaChange(e);
                }}
              />
              <FormText className="text-danger">{state.cinemaIdError}</FormText>
            </FormGroup>
            <FormGroup>
              <FormControl
                id="seatRows"
                className="add-new-form"
                type="number"
                placeholder="Number Of Rows"
                value={state.seatRows.toString()}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleNumRowsChange(e)}
                }              />
              <FormText className="text-danger">{state.seatRowsError}</FormText>
            </FormGroup>
            <FormGroup>
              <FormControl
                id="numberOfSeats"
                className="add-new-form"
                type="number"
                placeholder="Number Of Seats"
                value={state.numberOfSeats.toString()}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleNumSeatsChange(e)}
                }
                max="36"
              />
              <FormText className="text-danger">
                {state.numOfSeatsError}
              </FormText>
            </FormGroup>
            <Button
              type="submit"
              className="btn-add-new"
              disabled={state.submitted || !state.canSubmit}
              block
            >
              Add
            </Button>
          </form>
        </Col>
      </Row>
      <Row className="mt-2">
        <Col className="justify-content-center align-content-center">
          <h1 className="form-header">Auditorium Preview</h1>
          <div>
            <Row className="justify-content-center mb-4">
              <div className="text-center text-white font-weight-bold cinema-screen">
                CINEMA SCREEN
              </div>
            </Row>
            <Row className="justify-content-center">
              <table className="table-cinema-auditorium">
                <tbody>{renderRows(state.seatRows, state.numberOfSeats)}</tbody>
              </table>
            </Row>
          </div>
        </Col>
      </Row>
    </Container>
  );
};
export default withRouter(NewAuditorium);
