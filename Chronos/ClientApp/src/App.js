import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { TodoData } from './components/TodoData';
import { Import } from './components/Import';
import { WorkBlockData } from './components/WorkBlockData';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <Route path='/fetch-data' component={FetchData} />
        <Route path='/todo' component={TodoData} />
        <Route path='/import' component={Import} />
        <Route path='/work-block' component={WorkBlockData} />
      </Layout>
    );
  }
}
