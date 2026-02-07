package frc.robot.webserver;

import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.nio.charset.StandardCharsets;
import java.util.concurrent.Executors;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpServer;

import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import frc.robot.subsystems.Vision;

/**
 * Web server for vision diagnostics on RoboRIO.
 * 
 * Provides a web interface on port 8082 for monitoring:
 * - PhotonVision camera status
 * - AprilTag detection
 * - Ball/object detection
 * - Pose estimation
 * - Camera latency and performance
 * 
 * Access: http://10.TE.AM.2:8082 or http://roborio-TEAM-frc.local:8082
 */
public class VisionWebServer {
    
    private static final int PORT = 8082;
    private HttpServer server;
    @SuppressWarnings("unused")
    private Vision visionSubsystem;
    private boolean running = false;
    
    /**
     * Create a new vision web server.
     * 
     * @param vision The vision subsystem to monitor
     */
    public VisionWebServer(Vision vision) {
        this.visionSubsystem = vision;
    }
    
    /**
     * Start the web server.
     * 
     * @return true if started successfully, false otherwise
     */
    public boolean start() {
        try {
            server = HttpServer.create(new InetSocketAddress(PORT), 0);
            server.createContext("/", new DashboardHandler());
            server.createContext("/api/vision", new VisionDataHandler());
            server.createContext("/api/status", new StatusHandler());
            server.setExecutor(Executors.newFixedThreadPool(2));
            server.start();
            running = true;
            
            System.out.println("Vision Web Server started on port " + PORT);
            SmartDashboard.putBoolean("WebServer/Running", true);
            SmartDashboard.putNumber("WebServer/Port", PORT);
            
            return true;
        } catch (IOException e) {
            System.err.println("Failed to start web server: " + e.getMessage());
            SmartDashboard.putBoolean("WebServer/Running", false);
            return false;
        }
    }
    
    /**
     * Stop the web server.
     */
    public void stop() {
        if (server != null) {
            server.stop(0);
            running = false;
            SmartDashboard.putBoolean("WebServer/Running", false);
            System.out.println("Web server stopped");
        }
    }
    
    /**
     * Check if the web server is running.
     * 
     * @return true if running
     */
    public boolean isRunning() {
        return running;
    }
    
    /**
     * Handler for the main dashboard page.
     */
    private class DashboardHandler implements HttpHandler {
        @Override
        public void handle(HttpExchange exchange) throws IOException {
            String html = generateDashboardHTML();
            byte[] response = html.getBytes(StandardCharsets.UTF_8);
            
            exchange.getResponseHeaders().set("Content-Type", "text/html; charset=UTF-8");
            exchange.sendResponseHeaders(200, response.length);
            
            try (OutputStream os = exchange.getResponseBody()) {
                os.write(response);
            }
        }
    }
    
    /**
     * Handler for vision data API endpoint.
     */
    private class VisionDataHandler implements HttpHandler {
        @Override
        public void handle(HttpExchange exchange) throws IOException {
            String json = generateVisionJSON();
            byte[] response = json.getBytes(StandardCharsets.UTF_8);
            
            exchange.getResponseHeaders().set("Content-Type", "application/json");
            exchange.getResponseHeaders().set("Access-Control-Allow-Origin", "*");
            exchange.sendResponseHeaders(200, response.length);
            
            try (OutputStream os = exchange.getResponseBody()) {
                os.write(response);
            }
        }
    }
    
    /**
     * Handler for status API endpoint.
     */
    private class StatusHandler implements HttpHandler {
        @Override
        public void handle(HttpExchange exchange) throws IOException {
            String json = generateStatusJSON();
            byte[] response = json.getBytes(StandardCharsets.UTF_8);
            
            exchange.getResponseHeaders().set("Content-Type", "application/json");
            exchange.getResponseHeaders().set("Access-Control-Allow-Origin", "*");
            exchange.sendResponseHeaders(200, response.length);
            
            try (OutputStream os = exchange.getResponseBody()) {
                os.write(response);
            }
        }
    }
    
    /**
     * Generate the main dashboard HTML.
     */
    private String generateDashboardHTML() {
        // TODO: test this directory on an actual robot
        return Files.readString("src\\main\\java\\frc\\robot\\webserver\\dashboard.html");
    }
    
    /**
     * Generate vision data as JSON.
     */
    private String generateVisionJSON() {
        StringBuilder json = new StringBuilder();
        json.append("{");
        
        // Camera data
        json.append("\"cameras\":[");
        json.append("{");
        json.append("\"name\":\"Camera_0\",");
        json.append("\"connected\":").append(SmartDashboard.getBoolean("Vision/Camera_0/Enabled", false)).append(",");
        json.append("\"latency\":").append(SmartDashboard.getNumber("Vision/Camera_0/Latency", 0.0)).append(",");
        json.append("\"fps\":30,");
        json.append("\"hasTargets\":").append(SmartDashboard.getNumber("Vision/AprilTag/Count", 0) > 0).append(",");
        json.append("\"targetCount\":").append((int)SmartDashboard.getNumber("Vision/AprilTag/Count", 0));
        json.append("},");
        json.append("{");
        json.append("\"name\":\"banana_1\",");
        json.append("\"connected\":").append(SmartDashboard.getBoolean("Vision/Camera_1/Enabled", false)).append(",");
        json.append("\"latency\":").append(SmartDashboard.getNumber("Vision/Camera_1/Latency", 0.0)).append(",");
        json.append("\"fps\":30,");
        json.append("\"hasTargets\":").append(SmartDashboard.getBoolean("Vision/Ball/Visible", false)).append(",");
        json.append("\"targetCount\":").append(SmartDashboard.getBoolean("Vision/Ball/Visible", false) ? 1 : 0);
        json.append("}");
        json.append("],");
        
        // AprilTag data
        json.append("\"apriltag\":{");
        json.append("\"count\":").append((int)SmartDashboard.getNumber("Vision/AprilTag/Count", 0)).append(",");
        json.append("\"bestId\":").append((int)SmartDashboard.getNumber("Vision/AprilTag/BestID", -1)).append(",");
        json.append("\"distance\":").append(SmartDashboard.getNumber("Vision/AprilTag/BestDistance", 0.0)).append(",");
        json.append("\"yaw\":").append(SmartDashboard.getNumber("Vision/AprilTag/BestYaw", 0.0));
        json.append("},");
        
        // Object detection data
        json.append("\"object\":{");
        json.append("\"visible\":").append(SmartDashboard.getBoolean("Vision/Ball/Visible", false)).append(",");
        json.append("\"yaw\":").append(SmartDashboard.getNumber("Vision/Ball/Yaw", 0.0)).append(",");
        json.append("\"pitch\":").append(SmartDashboard.getNumber("Vision/Ball/Pitch", 0.0)).append(",");
        json.append("\"area\":").append(SmartDashboard.getNumber("Vision/Ball/Area", 0.0));
        json.append("},");
        
        // Pose estimation data
        json.append("\"pose\":{");
        json.append("\"hasVisionPose\":").append(SmartDashboard.getBoolean("Vision/HasVisionPose", false)).append(",");
        json.append("\"x\":").append(SmartDashboard.getNumber("Vision/EstimatedPose/X", 0.0)).append(",");
        json.append("\"y\":").append(SmartDashboard.getNumber("Vision/EstimatedPose/Y", 0.0)).append(",");
        json.append("\"rotation\":").append(SmartDashboard.getNumber("Vision/EstimatedPose/Rotation", 0.0)).append(",");
        json.append("\"timestamp\":").append(SmartDashboard.getNumber("Vision/PoseTimestamp", 0.0));
        json.append("}");
        
        json.append("}");
        return json.toString();
    }
    
    /**
     * Generate status data as JSON.
     */
    private String generateStatusJSON() {
        return String.format("{\"running\":%b,\"port\":%d}", running, PORT);
    }
}