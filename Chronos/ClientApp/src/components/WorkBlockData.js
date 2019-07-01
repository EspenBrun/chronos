import React, { Component } from 'react';
import Moment from 'moment';
import moment = require('moment');

export class WorkBlockData extends Component {
  static displayName = WorkBlockData.name;

  constructor (props) {
    super(props);
    this.state = { workBlocks: [], loading: true };

    fetch('api/import')
      .then(response => response.json())
      .then(data => {
        this.setState({ workBlocks: data, loading: false });
      });
  }

  static renderWorkBlockTable (workBlocks) {
    return (
      <div>
        <h5>Entries: {workBlocks.length}</h5>
        <table className='table table-striped'>
          <thead>
            <tr>
              <th>In</th>
              <th>Out</th>
              <th>Worked</th>
            </tr>
          </thead>
          <tbody>
            {workBlocks.map(workBlock =>
              <tr key={workBlock.id}>
                <td>{Moment(workBlock.in).format("full")}</td>
                <td>{Moment(workBlock.out).format("full")}</td>
                <td>{Moment(workBlock.worked).format("full")}</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    );
  }

  render () {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : WorkBlockData.renderWorkBlockTable(this.state.workBlocks);

    return (
      <div>
        <h1>WorkBlocks</h1>
        {contents}
      </div>
    );
  }
}
