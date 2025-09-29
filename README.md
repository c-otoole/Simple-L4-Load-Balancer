# Simple Layer 4 Load Balancer
SimpleLB is a lightweight Layer 4 (TCP) load balancer written in .NET Framework 4.7.2.
It accepts client connections on a single port and forwards them to multiple backend servers.

The solution was built for a technical Audition Challenge with limited time, so it focuses on clarity and correctness. With more time, I would extend it into a production-ready service (see Future Improvements).


## Features
- Accepts multiple concurrent client connections
- Distributes requests using round-robin across backends
- Health checks detect when backends go UP or DOWN
- Unhealthy backends are automatically removed from rotation until they recover
- Includes a sample backend server for local testing

## Getting Started

## Prerequisites
- Visual Studio 2019 (or later).
- .NET Framework 4.7.2 development tools installed.

### Clone the repository
```bash
git clone https://github.com/c-otoole/Simple-L4-Load-Balancer
cd SimpleLB
```

### Quick Demo
Start sample backend servers

A simple backend server can be found here - https://github.com/c-otoole/BackendServer

Run two instances on ports 8001 and 8002:

### First terminal
<img width="1125" height="216" alt="image" src="https://github.com/user-attachments/assets/953abc25-444f-466f-a2c7-bae7343b66f1" />

### Second terminal
<img width="1110" height="221" alt="image" src="https://github.com/user-attachments/assets/73ddb0c5-fec8-4ef2-9381-e2bb797a0dc4" />

Start the load balancer

Run the load balancer, which listens on port 8080 by default:

You should see output like:

<img width="1109" height="260" alt="image" src="https://github.com/user-attachments/assets/5f05e6fc-e7e2-400b-bd5d-1900c9c332ed" />

Connect through the load balancer
<img width="1095" height="624" alt="image" src="https://github.com/user-attachments/assets/fae688b8-89cf-4cce-a637-368e3fc07ede" />

<img width="521" height="237" alt="image" src="https://github.com/user-attachments/assets/802ee7ac-9bde-438a-9952-33b8f46609de" />


## Future Improvements

With more time, I would extend the project by:
-	Adding alternative balancing strategies (least connections, weighted round robin, IP hash)
-	Implementing TLS support for secure connections
-	Integrating structured logging instead of plain console output
-	Adding metrics (connections/sec, active sessions, backend health history)

## Notes
- This project demonstrates the fundamentals of a TCP load balancer in a clear, testable way
- The included backend server makes it easy to demo locally












