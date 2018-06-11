import React, { Component } from 'react';
import { Button, Col, Grid, Row } from 'react-bootstrap';
import { NavMenu } from './NavMenu';

export class Layout extends Component {
  displayName = Layout.name

    render() {

    return (
      <Grid fluid>
          <Row>
                <NavMenu />
                    <header className="masthead">
                        <div className="container">
                            <div className="intro-text">
                                <div className="intro-lead-in">Welcome To Musiquizza!</div>
                            <div className="intro-heading text-uppercase">For the Musically challenged</div>
                            <Button bsStyle="warning" bsSize="large" className="playBtn" href="#lyrics">Play the Game!</Button>
                            </div>
                        </div>
                    </header>
            </Row>
            <Row>
                <Col sm={12}>
                    {this.props.children}
                </Col>
            </Row>
      </Grid>
    );
  }
}
