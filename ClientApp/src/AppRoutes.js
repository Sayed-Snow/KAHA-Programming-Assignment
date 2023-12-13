import { Counter } from "./components/Counter";
import { WeatherForecast } from "./components/WeatherForecast";
import { Home } from "./components/Home";
import TableComponent from "./components/Table";
import CountryInfo from "./components/CountryInfo";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/country',
    element: <TableComponent />
  },
  {
    path: `/country/:country`,
    element: <CountryInfo />
  },
  {
    path: '/weather-forecast',
    element: <WeatherForecast />
  }
];

export default AppRoutes;
