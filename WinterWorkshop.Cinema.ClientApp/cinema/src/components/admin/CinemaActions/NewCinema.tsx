import React, { useState } from "react";
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
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCouch } from "@fortawesome/free-solid-svg-icons";

interface IState {
  name: string;
  nameError: string;
  auditName: string;
  seatRows: number;
  numberOfSeats: number;
  auditNameError: string;
  seatRowsError: string;
  numOfSeatsError: string;
  submitted: boolean;
  canSubmit: boolean;
}

const NewCinema: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    name: "",
    nameError: "",
    auditName: "",
    seatRows: 0,
    numberOfSeats: 0,
    auditNameError: "",
    seatRowsError: "",
    numOfSeatsError: "",
    submitted: false,
    canSubmit: true,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    validate(id, value);
    setState((prevData) => ({...prevData, [id]:value}));
  };
 
  const handleCinemaNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setState({ ...state, name: e.target.value });
  };

  const handleAuditNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setState({ ...state, auditName: e.target.value });
  };
  
  const handleRowsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setState((prevData)=>({...prevData, seatRows:Number(value)}));
    validate(id, value);
  }; 

  const handleSeatsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setState((prevData)=>({...prevData, numberOfSeats:Number(value)}));
    validate(id, value);
  };
  
  const validate = (id: string, value: string) => {
    if (id === "name") {
      if (value === "") {
        setState((prevData)=>({...prevData,   nameError: "Fill in cinema name",
        canSubmit: false,}));
      } else {
        setState((prevData)=>({...prevData,  nameError: "", canSubmit: true}));
      }
    }
    if (id === "auditName") {
      if (value === "") {
        setState((prevData)=>({...prevData, auditNameError: "Fill in auditorium name",
        canSubmit: false,}));
      } else {
        setState((prevData)=>({...prevData,auditNameError: "", canSubmit: true}));
      }
    } else if (id === "numberOfSeats") {
      const seatsNum = +value;
      if (seatsNum > 20 || seatsNum < 3) {
        setState((prevData)=>({...prevData,    numOfSeatsError: "Seats number can be in between 2 and 20.",
        canSubmit: false,}));
      } else {
        setState((prevData)=>({...prevData, numOfSeatsError: "", canSubmit: true}));
      }
    } else if (id === "seatRows") {
      const seatsNum = +value;
      if (seatsNum > 20 || seatsNum < 3) {
        setState((prevData)=>({...prevData,    seatRowsError: "Seats number can be in between 2 and 20.",
        canSubmit: false,}));
      } else {
        setState((prevData)=>({...prevData,   seatRowsError: "", canSubmit: true}));
      }
    }
  };

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setState({ ...state, submitted: true });
    if (state.name) {
      addCinema();
    } else {
      NotificationManager.error("Please fill in data");
      setState({ ...state, submitted: false });
    }
  };

  const addCinema = () => {
    const data = {
      name: state.name,
      numberOfSeats: +state.numberOfSeats,
      seatRows: +state.seatRows,
      auditName: state.auditName,
    };

    const requestOptions = {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("jwt")}`,
      },
      body: JSON.stringify(data),
    };

    fetch(`${serviceConfig.baseURL}/api/cinemas`, requestOptions)
      .then((response) => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.statusText;
      })
      .then((result) => {
        NotificationManager.success("Successfuly added cinema!");
        props.history.push(`AllCinemas`);
      })
      .catch((response) => {
        NotificationManager.error(response.message || response.statusText);
        setState({ ...state, submitted: false });
      });
  };

  const renderRows = (rows: number, seats: number) => {
    const rowsRendered: JSX.Element[] = [];
    for (let i = 0; i < rows; i++) {
      rowsRendered.push(<tr key={i}>{renderSeats(seats, i)}</tr>);
    }
    return rowsRendered;
  };

  const renderSeats = (seats: number, row: React.Key) => {
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

  return (
    <Container>
      <Row>
        <Col>
          <h1 className="form-header">Add New Cinema</h1>
          <form onSubmit={handleSubmit}>
            <FormGroup>
              <FormControl
                id="name"
                type="text"  
                placeholder="Cinema Name"
                value={state.name}
                className="add-new-form"
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleCinemaNameChange(e)}
                }
              />
              <FormText className="text-danger">{state.nameError}</FormText>
              <h1 className="form-header">Add Auditorium For Cinema</h1>
              <FormControl
                id="auditName"
                type="text"
                placeholder="Auditorium Name"
                value={state.auditName}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleAuditNameChange(e)}
                }
                className="add-new-form"
              />
              <FormText className="text-danger">
                {state.auditNameError}
              </FormText>
              <FormControl
                id="seatRows"
                className="add-new-form"
                type="number"
                placeholder="Number Of Rows"
                value={state.seatRows.toString()}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleRowsChange(e)}
                }            
              />
              <FormText className="text-danger">{state.seatRowsError}</FormText>
              <FormControl
                id="numberOfSeats"
                className="add-new-form"
                type="number"
                placeholder="Number Of Seats"
                value={state.numberOfSeats.toString()}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => 
                  {handleSeatsChange(e)}
                }
                max="36"
              />
              <FormText className="text-danger">
                {state.numOfSeatsError}
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
      <Row className="mt-2">
        <Col className="justify-content-center align-content-center">
          <h1 className="form-header">Auditorium Preview</h1>
          <div>
            <Row className="justify-content-center">
              <table className="table-cinema-auditorium">
                <tbody>{renderRows(state.seatRows, state.numberOfSeats)}</tbody>
              </table>
            </Row>
            <Row className="justify-content-center mb-4">
              <div className="text-center text-white font-weight-bold cinema-screen">
                CINEMA SCREEN
              </div>
            </Row>
          </div>
        </Col>
      </Row>
    </Container>
  );
};

export default withRouter(NewCinema);
