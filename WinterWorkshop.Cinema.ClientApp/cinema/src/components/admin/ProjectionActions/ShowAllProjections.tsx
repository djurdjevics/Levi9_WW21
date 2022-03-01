import React, { useEffect, useState } from "react";
import { NotificationManager } from "react-notifications";
import { serviceConfig } from "../../../appSettings";
import { Row, Table } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faTrash } from "@fortawesome/free-solid-svg-icons";
import Spinner from "../../Spinner";
import { IProjection } from "../../../models";
import { getProjections, deleteProjection, checkIfTicketExistForProjection } from "../../APICommunication";
interface IState {
  projections: IProjection[];
  isLoading: boolean;
}

const ShowAllProjections: React.FC = (props: any) => {
  const [state, setState] = useState<IState>({
    projections: [
      {
        id: "",
        movieId: "",
        movieTitle: "",
        auditoriumName: "",
        projectionTime: "",
      },
    ],
    isLoading: true,
  });

  useEffect(() => {
    getAllProjections();
  }, []);

  const getAllProjections = () => {
    setState((prevData) => ({...prevData, isLoading:true}));
    getProjections().then((data) => {
      if (data) {
        setState((prevData) => ({...prevData, projections: data, isLoading: false}));
      }
    })
    .catch((response) => {
      setState((prevData) => ({...prevData, isLoading: false}));
      NotificationManager.error(response.message || response.statusText);
    });
  };

  const removeProjection = (id: string) => {
    setState((prevData) => ({...prevData, isLoading: true}));
    checkIfTicketExistForProjection(id).then((data)=>{
      if(data)
      {
        console.log(data);
        if(data.length === 0)
        {
          deleteProjection(id).then((data) => {
            if(data)
            {
              NotificationManager.success("Successfully deleted projection.");
            }
          }).catch((response) => {
              NotificationManager.error(response.message || response.statusText);
              setState((prevData) => ({...prevData, isLoading: false}));
            });
        }
        else
        {
          NotificationManager.error("Projection can not be deleted, because there are purchased ticket for this projection.")
        }
        setTimeout(() => window.location.reload(), 1000);
      }
    })
    
  };

  const fillTableWithDaata = () => {
    return state.projections.map((projection) => {
      return (
        <tr key={projection.id}>
          <td width="18%">{projection.movieTitle}</td>
          <td width="18%">{projection.auditoriumName}</td>
          <td width="18%">{projection.projectionTime}</td>
          <td
            width="5%"
            className="text-center cursor-pointer"
            onClick={() => editProjection(projection.id)}
          >
            <FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit} />
          </td>
          <td
            width="5%"
            className="text-center cursor-pointer"
            onClick={() => removeProjection(projection.id)}
          >
            <FontAwesomeIcon
              className="text-danger mr-2 fa-1x"
              icon={faTrash}
            />
          </td>
        </tr>
      );
    });
  };

  const editProjection = (id: string) => {
    props.history.push(`editProjection/${id}`);
  };

  const rowsData = fillTableWithDaata();
  const table = (
    <Table striped bordered hover size="sm" variant="dark">
      <thead>
        <tr>
          <th>Movie Title</th>
          <th>Auditorium Name</th>
          <th>Projection Time</th>
          <th></th>
          <th></th>
        </tr>
      </thead>
      <tbody>{rowsData}</tbody>
    </Table>
  );
  const showTable = state.isLoading ? <Spinner></Spinner> : table;
  return (
    <React.Fragment>
      <Row className="no-gutters pt-2 projections-title">
        <h1 className="form-header form-heading projections-title">All Projections</h1>
      </Row>
      <Row className="no-gutters pr-5 pl-5">{showTable}</Row>
    </React.Fragment>
  );
};

export default ShowAllProjections;
