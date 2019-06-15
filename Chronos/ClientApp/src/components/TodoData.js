import React, { Component } from 'react';

export class TodoData extends Component {
  static displayName = TodoData.name;

  constructor (props) {
    super(props);
    this.state = { todos: [], loading: true };

    fetch('api/todoitems')
      .then(response => response.json())
      .then(data => {
        this.setState({ todos: data, loading: false });
      });
  }

  static renderTodosTable (todos) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Name</th>
            <th>Notes</th>
            <th>Done</th>
            <th>Difficulty</th>
            <th>Category</th>
            <th>Tags</th>
          </tr>
        </thead>
        <tbody>
          {todos.map(todos =>
            <tr key={todos.id}>
              <td>{todos.name}</td>
              <td>{todos.notes}</td>
              <td>{todos.done}</td>
              <td>{todos.difficulty}</td>
              <td>{todos.category}</td>
              <td>{todos.tag}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : TodoData.renderTodosTable(this.state.todos);

    return (
      <div>
        <h1>Todos</h1>
        <p>This component demonstrates fetching data from the database.</p>
        {contents}
      </div>
    );
  }
}
