import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Nav, Navbar, NavItem } from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';


export class NavMenu extends Component {
    displayName = NavMenu.name

    render() {
        return (
            <Navbar fixedTop inverse collapseOnSelect expand="lg" id="mainNav">
                <Navbar.Header>
                    <Navbar.Brand>
                        <Link to={'/'}>Musiquizza</Link>
                    </Navbar.Brand>
                    <Navbar.Toggle />
                </Navbar.Header>
                <Navbar.Collapse>
                    <Nav pullRight>
                        <LinkContainer to={'/'} exact>
                            <NavItem>
                                Play Game
                            </NavItem>
                        </LinkContainer>
                        <LinkContainer to={'/admin'}>
                            <NavItem>
                                Admin
                            </NavItem>
                        </LinkContainer>
                    </Nav>
                </Navbar.Collapse>
            </Navbar>
        );
    }
}
