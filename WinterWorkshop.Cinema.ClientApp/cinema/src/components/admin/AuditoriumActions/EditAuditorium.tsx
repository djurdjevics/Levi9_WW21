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
import { ICinema } from "../../../models";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCouch } from "@fortawesome/free-solid-svg-icons";

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

const EditAuditorium: React.FC = (props: any): JSX.Element => {
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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setState({ ...state, [id]: value });
    validate(id, value);
  };

  const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setState({ ...state, auditName: e.target.value });
  };
  
  const handleNumRowsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setState({ ...state, seatRows: Number(e.target.value) });
  }; 

  const handleNumSeatsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setState({ ...state, numberOfSeats: Number(e.target.value) });
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setState((prevState)=>({ ...prevState, submitted: true }));
    if (
      state.auditName &&
      state.numberOfSeats &&
      state.cinemaId &&
      state.seatRows
    ) {
      editAuditorium();
    } else {
      NotificationManager.error("Please fill form with data.");
      setState((prevState)=>({ ...prevState, submitted: false }));
    }
  };

  const validate = (id: string, value: string | null) => {
    if (id === "auditName") {
      if (value === "") {
        setState((prevState)=>({ ...prevState,
          auditNameError: "Fill in auditorium name",
          canSubmit: false,
        }));
      } else {
        setState((prevState)=>({ ...prevState, auditNameError: "", canSubmit: true }));
      }
    } else if (id === "numberOfSeats" && value) {
      const seatsNum = +value;
      if (seatsNum > 20 || seatsNum < 1) {
        setState((prevState)=>({ ...prevState,
          numOfSeatsError: "Seats number can be in between 1 and 20.",
          canSubmit: false,
        }));
      } else {
        setState((prevState)=>({ ...prevState, numOfSeatsError: "", canSubmit: true }));
      }
    } else if (id === "seatRows" && value) {
      const seatsNum = +value;
      if (seatsNum > 20 || seatsNum < 1) {
        setState((prevState)=>({ ...prevState,
          seatRowsError: "Seats number can be in between 1 and 20.",
          canSubmit: false,
        }));
      } else {
        setState((prevState)=>({ ...prevState, seatRowsError: "", canSubmit: true }));
      }
    } else if (id === "cinemaId") {
      if (!value) {
        setState((prevState)=>({ ...prevState,
          cinemaIdError: "Please chose cinema from dropdown list.",
          canSubmit: false,
        }));
      } else {
        setState((prevState)=>({ ...prevState, cinemaIdError: "", canSubmit: true }));
      }
    }
  };

  const editAuditorium = () => {
    const idFromUrl = window.location.pathname.split("/");
    const id = idFromUrl[3];

    const data = {
      cinemaId: state.cinemaId,
      numberOfSeats: +state.numberOfSeats,
      seatRows: +state.seatRows,
      auditName: state.auditName,
    };

    const requestOptions = {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
      body: JSON.stringify(data),
    };

    fetch(
      `${serviceConfig.baseURL}/api/auditoriums/${id}`,
      requestOptions
    )
      .then((response) => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.statusText;
      })
      .then((result) => {
        NotificationManager.success("Successfuly edited auditorium!");
        props.history.push("AllAuditoriums");
      })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState((prevState)=>({ ...prevState, submitted: false }));
      });
  };

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
          setState((prevState)=>({ ...prevState, cinemas: data }));
        }
      })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState((prevState)=>({ ...prevState, submitted: false }));
      });
  };

  useEffect(() => {
    getCinemas();
  }, []);

  const onCinemaChange = (cinemas: ICinema[]) => {
    if (cinemas[0]) {
      setState((prevState)=>({ ...prevState, cinemaId: cinemas[0].id }));
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
          <h1 className="form-header">Edit Auditorium</h1>
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
              />
              <FormText className="text-danger">
                {state.auditNameError}
              </FormText>
            </FormGroup>
            <FormGroup>
              <Typeahead
                labelKey="name"
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
                type="number"
                placeholder="Number Of Rows"
                value={state.seatRows.toString()}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleNumRowsChange(e)}
                }                
                />
              <FormText className="text-danger">{state.seatRowsError}</FormText>
            </FormGroup>
            <FormGroup>
              <FormControl
                id="numberOfSeats"
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
              disabled={state.submitted || !state.canSubmit}
              block
            >
              Edit Auditorium
            </Button>
          </form>
        </Col>
      </Row>
      <Row className="mt-2">
        <Col className="justify-content-center align-content-center">
          <h1>Auditorium Preview</h1>
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

export default withRouter(EditAuditorium);
