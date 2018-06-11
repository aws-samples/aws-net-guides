import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Lyrics } from './components/Lyrics';
import { Quiz } from './components/Quiz';

export default class App extends Component {
  displayName = App.name

  render() {
      return (
 
        <Layout>
            <section id="lyrics">
                <div className="container">
                    <Route exact path='/' component={Lyrics} />
                      <Route exact path='/' component={Quiz} />
                      <Route path='/admin' component={Quiz} />
                </div>
            </section>
        </Layout>
             
           
    );
  }
}
