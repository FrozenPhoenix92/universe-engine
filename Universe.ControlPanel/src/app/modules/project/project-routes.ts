import { Routes } from "@angular/router";

export const ProjectRoutes = <Routes> [
	{
		path: "store-overview",
		loadChildren: () => import("./modules/store-overview/store-overview.module").then(x => x.StoreOverviewModule)
	}
];