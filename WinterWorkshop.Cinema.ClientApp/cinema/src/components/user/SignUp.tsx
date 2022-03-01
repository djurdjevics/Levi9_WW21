import React, { useEffect, useState } from "react";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../appSettings";
import { Button, Col, Container, FormControl, FormGroup, FormText, Row } from "react-bootstrap";
import { getUserName, getRole } from "../helpers/authCheck";
import { withRouter } from "react-router";
import { IProjection, IUser, IReservation } from "../../models";
import Header from "../Header";

interface IState {
  firstName: string;
  lastName: string;
  userName: string;
  role: string;
  firstNameError: string;
  lastNameError: string;
  userNameError: string;
  roleError: string;
  submitted: boolean;
  tags: string;
  canSubmit: boolean;
}

const SignUp: React.FC = (props:any) => {
  const [state, setState] = useState<IState>({
    firstName: "",
    lastName: "",
    userName: "",
    role: "1",
    firstNameError: "",
    lastNameError: "",
    userNameError: "",
    roleError: "",
    tags: "",
    submitted: false,
    canSubmit: true
  });

const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setState({...state, submitted: true});
    const {firstName, lastName, userName, role} = state;
    if(firstName && lastName && userName && role){
        addUser();
    }
    else
    {
        NotificationManager.error("Please fill in data");
        setState({ ...state, submitted: false });
    }
};

const castRole = (r) => {
  if(r === "1")
    return "user";
  else if(r === "2")
    return "super-user";
  else
    return "admin"
};

const addUser = () => {
    let role = castRole(state.role);
    const data = {
        FirstName: state.firstName,
        LastName: state.lastName,
        UserName: state.userName,
        Role: role
    };
    const requestOptions = {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("jwt")}`,
        },
        body: JSON.stringify(data),
      };
  
      fetch(`${serviceConfig.baseURL}/api/users`, requestOptions)
        .then(async (response) => {
          if (!response.ok) {
            return Promise.reject(await response.json());
          }
          return response.statusText;
        })
        .then((result) => {
          NotificationManager.success("Successfuly added user!");
          login();
          props.history.push(`Projection`);
        })
        .catch((response) => {
          NotificationManager.error(response.errorMessage);
          setState({ ...state, submitted: false });
        });
}

const login = () => {
 getToken(castRole(state.role));
 NotificationManager.success("Welcome, " + state.firstName);
};


const getToken = (
  Role: string
) => {
  const requestOptions = {
    method: "GET",
  };
  fetch(
    `${serviceConfig.baseURL}/get-token?role=${Role}&userName=${state.userName}`,
    requestOptions
  )
    .then((response) => {
      if (!response.ok) {
        return Promise.reject(response);
      }
      return response.json();
    })
    .then((data) => {
      if (data.token) {
        localStorage.setItem("jwt", data.token);
        setTimeout(() => {
          window.location.reload();
        }, 500);
      }
    })
    .catch((response) => {
      NotificationManager.error(response.message || response.statusText);
      setState({ ...state, submitted: false });
    });
};

const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target;
    setState({ ...state, [id]: value});
    validate(id, value);
  };

const validate = (id: string, value: string) => {
    if (id === "firstName") {
      if (value.trim() === "") {
        setState({
          ...state, 
          firstName: value,
          firstNameError: "Fill in first name.",
          canSubmit: false,
        });
      } else {
        setState({ ...state, firstName: value,  firstNameError: "", canSubmit: true });
      }
    }

    if (id === "lastName") {
        if (value.trim() === "") {
          setState({
            ...state,
            lastName: value,
            lastNameError: "Fill in last name.",
            canSubmit: false,
          });
        } else {
          setState({ ...state, lastName:value, lastNameError: "", canSubmit: true });
        }
      }

    if (id === "userName") {
       if (value.trim() === "" || value ==="." || value === "..") {
         setState({
           ...state,
           userName: value,
           userNameError: "Fill in user name.",
           canSubmit: false,
         });
       } else {
         setState({ ...state, userName:value, userNameError: "", canSubmit: true });
       }
     }
};

  return (
    <Container>
      <Row>
        <Col>
          <h1 className="form-header projections-title">Sign up</h1>
          <form onSubmit={handleSubmit}>
            <FormGroup>
                <FormControl
                id="firstName"
                type="text"
                placeholder="First Name"
                value={state.firstName}
                onChange={handleChange}
                className="add-new-form"
                />
                <FormText className="text-danger add-new-form">{state.firstNameError}</FormText>
            </FormGroup>

            <FormGroup>
                <FormControl
                id="lastName"
                type="text"
                placeholder="Last Name"
                value={state.lastName}
                onChange={handleChange}
                className="add-new-form"
                />
                <FormText className="text-danger add-new-form">{state.lastNameError}</FormText>
            </FormGroup>

            <FormGroup>
                <FormControl
                id="userName"
                type="text"
                placeholder="UserName"
                value={state.userName}
                onChange={handleChange}
                className="add-new-form"
                />
                <FormText className="text-danger add-new-form">{state.userNameError}</FormText>
            </FormGroup>
            <FormGroup>
              <FormControl
                as="select"
                className="add-new-form"
                placeholder="Role"
                id="role"
                value={state.role}
                onChange={handleChange}
              >
                <option value="1">User</option>
                <option value="2">Super-user</option>
                <option value="3">Admin</option>
              </FormControl>
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

export default withRouter(SignUp);
